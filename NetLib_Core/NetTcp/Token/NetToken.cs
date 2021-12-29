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

        public MemoryStream ReceiveStream()
        {
            //if (messageResolver.ReceivePacket(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred, out MemoryStream outputStream))
            //{
            //    return outputStream;
            //}

            return null;
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
                sendArgs.SetBuffer(data, 0, data.Length);

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
