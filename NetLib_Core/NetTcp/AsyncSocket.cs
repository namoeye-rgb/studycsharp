using System.Net.Sockets;

namespace NetLib
{
    public partial class AsyncSocket
    {
        private Socket socket = null;
        public Socket Socket {
            get {
                return socket;
            }
        }

        public volatile SocketError SocketState;

        public delegate void NewClient_CallBack(SocketAsyncEventArgs e);
        public delegate void ServerConnect_CallBack(SocketAsyncEventArgs e);
        public delegate void ReceiveComplete_CallBack(SocketAsyncEventArgs e);
        public delegate void SendComplete_CallBack(SocketAsyncEventArgs e);

        public ServerConnect_CallBack ConnectServer;
        public ReceiveComplete_CallBack ReceiveComplete;
        public SendComplete_CallBack SendComplete;
        public NewClient_CallBack AcceptClient;

        public void ReceiveAsync(object sender, SocketAsyncEventArgs e)
        {
            SocketState = e.SocketError;
            if (e.LastOperation == SocketAsyncOperation.Receive) {
                ReceiveComplete?.Invoke(e);
            }
        }

        public void SendAsync(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Send) {
                SendComplete?.Invoke(e);
            }
        }
    }
}
