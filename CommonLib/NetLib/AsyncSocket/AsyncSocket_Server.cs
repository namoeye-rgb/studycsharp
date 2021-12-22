using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetLib
{
    public partial class AsyncSocket
    {
        public void StartServer(int _port, int _maxConnetion)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += AcceptComplete;

            try
            {
                socket.Bind(localEndPoint);
                socket.Listen(_maxConnetion);
                socket.AcceptAsync(args);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                SocketState = SOCKET_STATE.ERROR;
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
