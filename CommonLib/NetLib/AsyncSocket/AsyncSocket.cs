using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetLib
{
    public enum SOCKET_STATE
    {
        NONE,
        SUCCESS,
        ERROR,
    }

    public partial class AsyncSocket
    {
        Socket socket = null;
        public Socket Socket {
            get {
                return socket;
            }
        }

        public volatile SOCKET_STATE SocketState;

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
            if (e.BytesTransferred == 0 && e.SocketError != SocketError.Success)
            {
                SocketState = SOCKET_STATE.ERROR;
            }

            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                ReceiveComplete?.Invoke(e);
            }
        }

        public void SendAsync(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Send)
            {
                SendComplete?.Invoke(e);
            }
        }
    }
}
