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
        IPEndPoint RemoteEP;
        public void StartClient(string _ip, int _port)
        {
            try
            {
                var ipAddress = IPAddress.Parse(_ip);
                RemoteEP = new IPEndPoint(ipAddress, _port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = RemoteEP;
                args.Completed += ConnectComplete;

                socket.ConnectAsync(args);

            }
            catch (Exception e)
            {
                SocketState = SOCKET_STATE.ERROR;
                Console.WriteLine(e.ToString());
            }
        }

        public bool ReConnectClient()
        {
            if (RemoteEP == null)
            {
                return false;
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = RemoteEP;
            args.Completed += ConnectComplete;

            socket.ConnectAsync(args);

            return true;
        }
        public void ConnectComplete(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                SocketState = SOCKET_STATE.SUCCESS;
            }
            ConnectServer?.Invoke(e);
        }
    }
}
