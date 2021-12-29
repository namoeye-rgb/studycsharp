using System.Net.Sockets;

namespace NetLib
{
    public partial class AsyncSocket
    {
        private Socket socket = null;
        public Socket Socket
        {
            get
            {
                return socket;
            }
        }

        public void SetSocket(Socket socket)
        {
            this.socket = socket;
        }

        public volatile SocketError SocketState;

        public delegate void AcceptClient_CallBack(SocketAsyncEventArgs e);
        public delegate void ServerConnect_CallBack(SocketAsyncEventArgs e);
        public delegate void ReceiveComplete_CallBack(SocketAsyncEventArgs e);
        public delegate void SendComplete_CallBack(SocketAsyncEventArgs e);

        public ServerConnect_CallBack ServerConnectCallback;
        public ReceiveComplete_CallBack ReceiveCompleteCallback;
        public SendComplete_CallBack SendCompleteCallback;
        public AcceptClient_CallBack AcceptClientCallback;

        public bool SendAsync(SocketAsyncEventArgs e)
        {
            var result = socket.SendAsync(e);
            if (result == false)
            {
                SendCompate(this, e);
            }
            return result;
        }

        public bool ReceiveAsync(SocketAsyncEventArgs e)
        {
            var result = socket.ReceiveAsync(e);
            if (result == false)
            {
                ReceiveComplete(this, e);
            }
            return result;
        }

        public void ReceiveComplete(object sender, SocketAsyncEventArgs e)
        {
            SocketState = e.SocketError;
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                ReceiveCompleteCallback?.Invoke(e);
            }
        }

        public void SendCompate(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Send)
            {
                SendCompleteCallback?.Invoke(e);
            }
        }
    }
}
