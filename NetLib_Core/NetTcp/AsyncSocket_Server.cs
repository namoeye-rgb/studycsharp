using System;
using System.Net;
using System.Net.Sockets;

namespace NetLib
{
    public partial class AsyncSocket
    {
        public void StartServer(int port, int listenCount)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += AcceptComplete;

            try {
                socket.Bind(localEndPoint);
                socket.Listen(listenCount);
                socket.AcceptAsync(args);

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                SocketState = SocketError.SocketError;
            }
        }

        public void AcceptComplete(object sender, SocketAsyncEventArgs e)
        {
            AcceptClientCallback?.Invoke(e);
            e.AcceptSocket = null;
            socket.AcceptAsync(e);
        }
    }
}
