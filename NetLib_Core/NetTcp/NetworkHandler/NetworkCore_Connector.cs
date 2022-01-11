using CommonLib_Core;
using NetLib;
using NetLib.Token;
using PacketLib_Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NetLib_Core.NetTcp.NetworkHandler
{
    internal class NetworkCore_Connector : NetworkCore_Base
    {
        //Client
        private Session session;

        private IPEndPoint remoteEP;

        public delegate void OnConnectedHandler(Session netToken);
        public OnConnectedHandler onConnected;

        public override void Init(NetworkConfig config, ILogger logger, Assembly excuteAssembly, string packetHandlerClassName, Func<Session> createSessionFunc)
        {
            NetType = NET_TYPE.Client;
            this.logger = logger;

            packetHandler = new PacketHandler();
            packetHandler.Initialize(logger, excuteAssembly, packetHandlerClassName);

            var ipAddress = IPAddress.Parse(config.ConnectIP);
            remoteEP = new IPEndPoint(ipAddress, config.ConnectPort);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.createSessionFunc = createSessionFunc;
        }

        public void RegisterHandler(OnConnectedHandler onConnectedHandler, OnRecvHandler onReceiveHadler, OnDisConnectHandler onDisConnectHandler)
        {
            onConnected += onConnectedHandler;
            onRecv += onReceiveHadler;
            onDisConnected += onDisConnectHandler;
        }

        public void Start_Client()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEP;
            args.Completed += ConnectComplete;

            socket.ConnectAsync(args);
        }

        private bool ReConnect()
        {
            if (remoteEP == null)
            {
                logger?.Error("not found remoteEP");
                return false;
            }

            socket?.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEP;
            args.Completed += ConnectComplete;

            socket.ConnectAsync(args);

            return true;
        }

        //클라이언트 용
        private void ConnectComplete(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                logger?.Warn("Fail Connect Server, SocketError : {0}, Remote EndPoint : {1}", e.SocketError, e.RemoteEndPoint);
                Thread.Sleep(5000);

                if (ReConnect() == false)
                {
                    logger?.Error("Not Found Remote Connect Info");
                    return;
                }
                return;
            }

            Func<SocketAsyncEventArgs> createEventArgsFunc = () =>
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] buffer = new byte[NetworkCoreConst.bufferSize];
                args.SetBuffer(buffer, 0, buffer.Length);

                return args;
            };

            session = createSessionFunc();

            SocketAsyncEventArgs sendArgs = createEventArgsFunc();
            sendArgs.Completed += SendComplete;
            sendArgs.UserToken = session;

            SocketAsyncEventArgs recvArgs = createEventArgsFunc();
            recvArgs.Completed += RecvComplete;
            recvArgs.UserToken = session;

            session.Init(e.ConnectSocket, recvArgs, sendArgs);
            session.Receive();

            onConnected?.Invoke(session);
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}
