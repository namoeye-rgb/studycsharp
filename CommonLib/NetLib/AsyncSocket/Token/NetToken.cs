using PacketLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetLib.Token
{
    public class NetToken : IToken
    {
        Socket socket;
        SocketAsyncEventArgs receiveArgs;
        SocketAsyncEventArgs sendArgs;

        Queue<byte[]> sendQueue = new Queue<byte[]>();
        object sendQueueObj = new object();

        MessageResolver messageResolver;

        IUserToken userToken;
        

        public Socket Socket {
            get {
                return socket;
            }
        }

        public SocketAsyncEventArgs ReceiveArgs {
            get {
                return receiveArgs;
            }
        }

        public SocketAsyncEventArgs SendArgs {
            get {
                return sendArgs;
            }
        }

        public void Init(Socket _socket, SocketAsyncEventArgs _receive, SocketAsyncEventArgs _send, NETCLIENT_TYPE _type)
        {
            socket = _socket;
            receiveArgs = _receive;
            sendArgs = _send;

            messageResolver = new MessageResolver();
            messageResolver.HeaderSize = (short)_type;
        }

        public void Close()
        {
            socket.Close();
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

        public void Ready_Receive()
        {
            try
            {
                socket?.ReceiveAsync(receiveArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public MemoryStream ReceivePacket()
        {
            if (messageResolver.ReceivePacket(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred, out MemoryStream outputStream))
            {
                return outputStream;
            }

            return null;
        }

        public void Ready_Send()
        {
            socket?.SendAsync(sendArgs);
        }

        //Send Packet
        public void SendPacket(byte[] _buffer)
        {
            lock (sendQueueObj)
            {

                if (sendQueue.Count <= 0)
                {
                    sendQueue.Enqueue(_buffer);
                    StartSend();
                    return;
                }

                sendQueue.Enqueue(_buffer);
            }
        }

        private void StartSend()
        {
            if (sendQueue.Count <= 0)
                return;

            lock (sendQueueObj)
            {
                var data = sendQueue.Peek();
                sendArgs.SetBuffer(data, 0, data.Length);
                socket.SendAsync(sendArgs);
            }
        }

        public void SendDequeue()
        {
            lock (sendQueueObj)
                sendQueue.Dequeue();

            if (sendQueue.Count > 0)
            {
                StartSend();
            }
        }

        #endregion
    }
}
