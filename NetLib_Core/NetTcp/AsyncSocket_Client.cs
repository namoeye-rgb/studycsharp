using System;
using System.Net;
using System.Net.Sockets;

namespace NetLib
{
    public partial class AsyncSocket
    {
        private IPEndPoint RemoteEP;

        private bool isReconnect;
        public void StartClient(string ip, int port, bool isReceonnect)
        {
            try
            {
                var ipAddress = IPAddress.Parse(ip);
                RemoteEP = new IPEndPoint(ipAddress, port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = RemoteEP;
                args.Completed += ConnectComplete;

                socket.ConnectAsync(args);

                isReconnect = isReceonnect;

            }
            catch (Exception e)
            {
                SocketState = SocketError.SocketError;
                Console.WriteLine(e.ToString());
            }
        }

        public bool ReConnectClient()
        {
            if (isReconnect == false)
            {
                return false;
            }

            if (RemoteEP == null)
            {
                return false;
            }

            socket?.Close();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = RemoteEP;
            args.Completed += ConnectComplete;

            socket.ConnectAsync(args);

            return true;
        }
        public void ConnectComplete(object sender, SocketAsyncEventArgs e)
        {
            SocketState = e.SocketError;
            ServerConnectCallback?.Invoke(e);
        }
    }
}
