using System;
using System.Net;
using System.Net.Sockets;

namespace NetLib
{
    public partial class AsyncSocket
    {
        public void StartServer(int _port, int _listenCount)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += AcceptComplete;

            try {
                socket.Bind(localEndPoint);
                socket.Listen(_listenCount);
                socket.AcceptAsync(args);

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                SocketState = SocketError.SocketError;
            }
        }

        public void AcceptComplete(object sender, SocketAsyncEventArgs e)
        {
            AcceptClient?.Invoke(e);
            e.AcceptSocket = null;
            socket.AcceptAsync(e);
        }
    }
}
