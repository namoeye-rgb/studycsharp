using CommonLib_Core;
using Google.Protobuf;
using PacketLib_Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace NetLib.Token
{
    public abstract class Session: IToken, INetSession
    {
        private Socket socket;
        private int disconnect = 0;

        private SocketAsyncEventArgs receiveArgs;
        private SocketAsyncEventArgs sendArgs;
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private object sendQueueObj = new object();
        private byte[] myBuffer;
        private int curPos;

        public abstract void OnConnectedHandler(Session netToken);
        public abstract void OnRecvHandler(INetSession userToken, short id, byte[] buffer);
        public abstract void OnDisConnectHandler(Session netToken);

        public Socket Socket
        {
            get
            {
                return socket;
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

        public void Init(Socket socket, SocketAsyncEventArgs receive, SocketAsyncEventArgs send)
        {
            this.socket = socket;
            receiveArgs = receive;
            sendArgs = send;

            myBuffer = new byte[receive.Count];
        }

        public void Close()
        {
            socket.Close();
        }


        #region 실제 패킷 처리 함수

        public void Receive()
        {
            socket.ReceiveAsync(receiveArgs);
        }

        public void ReceiveByteBuffer(Action<short, byte[]> onReceiveCallback, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred < 0)
            {
                return;
            }

            try
            {
                Array.Copy(e.Buffer, e.Offset, myBuffer, curPos, e.BytesTransferred);
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

                    byte[] packetBuffer = new byte[bodySize];
                    Array.Copy(myBuffer, readPos + PacketConst.HEADER_SIZE, packetBuffer, 0, bodySize);
                    var id = BitConverter.ToInt16(myBuffer, 0);
                    //하나의 패킷 데이터를 온전히 가져온다
                    onReceiveCallback(id, packetBuffer);

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


        //클라이언트 -> 서버 패킷 전송 함수
        public void SendPacket<T>(T sendPacket) where T : IMessage
        {
            var packetBuffer = PacketHandler.GetPacketToBytes(sendPacket);
            SendPacket(packetBuffer);
        }

        //Send Packet
        private void SendPacket(byte[] buffer)
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
                //TODO : 현재 버퍼 사이즈인 65536 이상일 경우 문제가 생기므로 추후 수정
                sendArgs.SetBuffer(sendArgs.Offset, data.Length);
                Array.Copy(data, 0, sendArgs.Buffer, sendArgs.Offset, data.Length);

                socket.SendAsync(sendArgs);
            }
        }

        public void DequeueSend()
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

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnect, 1) == 1)
            {
                return;
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        #endregion
    }
}
