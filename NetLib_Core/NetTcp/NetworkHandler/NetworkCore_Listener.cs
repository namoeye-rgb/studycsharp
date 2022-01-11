using CommonLib_Core;
using Google.Protobuf;
using NetLib.Token;
using PacketLib_Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace NetLib
{
    public partial class NetworkCore_Listener : NetworkCore_Base
    {
        private BufferManager bufferMgr;
        private SocketAsyncEventArgsPool receiveEventPool;
        private SocketAsyncEventArgsPool sendEventPool;

        private EndPoint localEndPoint;
        private NetworkConfig listenerConfig;

        public delegate void OnAcceptHandler(Session netToken);
        public OnAcceptHandler onAccept;


        public override void Init(NetworkConfig config, ILogger logger, Assembly excuteAssembly, string packetHandlerClassName, Func<Session> createSessionFunc)
        {
            NetType = NET_TYPE.Server;

            this.listenerConfig = config;
            this.logger = logger;

            packetHandler = new PacketHandler();
            packetHandler.Initialize(logger, excuteAssembly, packetHandlerClassName);

            InitializePool(config.MaxConnection);

            localEndPoint = new IPEndPoint(IPAddress.Any, config.ListenPort);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.createSessionFunc = createSessionFunc;
        }

        public void RegisterHandler(
            OnAcceptHandler onAcceptHandler,            
            OnRecvHandler onReceiveHandler,
            OnDisConnectHandler onDisConnectHandler)
        {
            onAccept += onAcceptHandler;
            onRecv += onReceiveHandler;
            onDisConnected += onDisConnectHandler;
        }

        public void Start_Server()
        {
            try
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += AcceptComplete;

                socket.Bind(localEndPoint);
                socket.Listen(listenerConfig.BackLogCount);
                socket.AcceptAsync(args);

            }
            catch (Exception e)
            {
                logger?.Error(e.ToString());
            }
        }

        private void AcceptComplete(object sender, SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs receiveArgs = receiveEventPool.PopPool();
            SocketAsyncEventArgs sendArgs = sendEventPool.PopPool();

            Session token = receiveArgs.UserToken as Session;

            token.Init(e.AcceptSocket, receiveArgs, sendArgs);
            logger?.Debug($"Client Accept : {e.AcceptSocket.RemoteEndPoint}");
            onAccept?.Invoke(token);
            token.Receive();

            e.AcceptSocket = null;
            socket.AcceptAsync(e);
        }


        private void InitializePool(int maxConnection)
        {
            bufferMgr = new BufferManager((maxConnection * NetworkCoreConst.bufferSize) * 2, NetworkCoreConst.bufferSize);

            receiveEventPool = new SocketAsyncEventArgsPool(maxConnection);
            sendEventPool = new SocketAsyncEventArgsPool(maxConnection);

            for (int i = 0; i < maxConnection; ++i)
            {
                Session token = createSessionFunc();
                //recive Pool
                SocketAsyncEventArgs receive = new SocketAsyncEventArgs();
                bufferMgr.SetBuffer(receive);
                receive.UserToken = token;
                receive.Completed += RecvComplete;
                receiveEventPool.PushPool(receive);

                //send Pool
                SocketAsyncEventArgs send = new SocketAsyncEventArgs();
                bufferMgr.SetBuffer(send);
                send.UserToken = token;
                send.Completed += SendComplete;
                sendEventPool.PushPool(send);
            }
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}
