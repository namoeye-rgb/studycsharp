using PacketLib_Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace NetLib.Token
{
    public class NetToken : IToken
    {
        private AsyncSocket asyncSocket;
        private SocketAsyncEventArgs receiveArgs;
        private SocketAsyncEventArgs sendArgs;
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private object sendQueueObj = new object();
        private IUserToken userToken;
        private PacketHandler packetHandler;

        private byte[] myBuffer;
        private int curPos;

        public Socket Socket
        {
            get
            {
                return asyncSocket.Socket;
            }
        }

        public SocketAsyncEventArgs ReceiveArgs
        {
            get
            {
                return receiveArgs;
            }
        }

        public SocketAsyncEventArgs SendArgs
        {
            get
            {
                return sendArgs;
            }
        }

        public void Init(AsyncSocket socket, SocketAsyncEventArgs receive, SocketAsyncEventArgs send)
        {
            this.asyncSocket = socket;
            receiveArgs = receive;
            sendArgs = send;

            myBuffer = new byte[receive.Buffer.Length];
        }

        public void Close()
        {
            asyncSocket.Socket.Close();
        }

        public void SetUserToken(IUserToken token)
        {
            userToken = token;
        }

        public IUserToken GetUserToken()
        {
            return userToken;
        }


        #region 실제 패킷 처리 함수

        public bool Receive()
        {
            try
            {
                asyncSocket.ReceiveAsync(receiveArgs);
                return true;
            }
            catch (Exception e)
            {
                asyncSocket?.Socket.Close();
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void ReceiveByteBuffer(Action<IUserToken, byte[]> onReceiveCallback, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred < 0)
            {
                return;
            }

            try
            {
                Array.Copy(e.Buffer, e.Offset, myBuffer, curPos, e.Buffer.Length - e.Offset);
                //현재 위치를 받은 만큼 움직입니다
                curPos += e.BytesTransferred;

                int remainLength = curPos;
                int readPos = 0;
                while (true)
                {
                    //현재 위치가 헤더 사이즈보다 작으면 그냥 다음 또 받기위해 리턴
                    if (remainLength < PacketConst.HEADER_SIZE)
                    {
                        break;
                    }

                    //0000 0000 | 0000 0000 / 0000 0000 | 0000 0000 | 0000 0000 | 0000 0000 /
                    //        id(2byte)     /                 body(4byte)                   /
                    int bodySize = BitConverter.ToInt32(myBuffer, PacketConst.ID_SIZE);
                    //헤더 + 바디사이즈 길이 > 현재 위치값 보다 크면 다시 또 받기위해 return
                    //하나의 정사이즈가 최소한 들어와야 다음 처리를 한다는 의미
                    var packetSize = PacketConst.HEADER_SIZE + bodySize;
                    if (packetSize > curPos)
                    {
                        break;
                    }

                    using (MemoryStream ms = new MemoryStream(packetSize))
                    {
                        //id를 가져오고
                        ms.Write(myBuffer, 0, PacketConst.ID_SIZE);
                        //몸통 가져오고
                        ms.Write(myBuffer, PacketConst.HEADER_SIZE, bodySize);
                        ms.Flush();

                        //하나의 패킷 데이터를 온전히 가져온다
                        onReceiveCallback(GetUserToken(), ms.GetBuffer());

                    };

                    //여기서 myBuff를 손보긴 해야함
                    Array.Clear(myBuffer, readPos, packetSize);
                    readPos += packetSize;
                    remainLength -= packetSize;
                    curPos -= packetSize;
                }
            }
            finally
            {
                Array.Clear(e.Buffer, e.Offset, e.BytesTransferred);
            }
        }

        //Send Packet
        public void SendPacket(byte[] buffer)
        {
            lock (sendQueueObj)
            {

                if (sendQueue.Count <= 0)
                {
                    sendQueue.Enqueue(buffer);
                    StartSend();
                    return;
                }

                sendQueue.Enqueue(buffer);
            }
        }

        private void StartSend()
        {
            if (sendQueue.Count <= 0)
            {
                return;
            }

            lock (sendQueueObj)
            {
                var data = sendQueue.Peek();
                sendArgs.SetBuffer(sendArgs.Offset, data.Length);
                Array.Copy(data, 0, sendArgs.Buffer, sendArgs.Offset, data.Length);

                asyncSocket.SendAsync(sendArgs);
            }
        }

        public void SendDequeue()
        {
            lock (sendQueueObj)
            {
                if (sendQueue.Count > 0)
                {
                    sendQueue.Dequeue();
                    StartSend();
                }
            }
        }

        #endregion
    }
}
