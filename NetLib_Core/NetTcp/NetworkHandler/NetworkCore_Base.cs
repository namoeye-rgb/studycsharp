using CommonLib_Core;
using NetLib;
using NetLib.Token;
using PacketLib_Core;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace NetLib
{
    public enum NET_TYPE
    {
        Client,
        Server,
    }

    public class NetworkConfig
    {
        public int MaxConnection;
        public int BackLogCount;

        public string ConnectIP;
        public int ConnectPort;
        public int ListenPort;
    }

    public class NetworkCoreConst
    {
        public const int bufferSize = 65535;
    }

    public abstract class NetworkCore_Base
    {
        protected ILogger logger;
        public NET_TYPE NetType { get; protected set; }

        protected Socket socket;                                                                       
        protected PacketHandler packetHandler;

        public Func<Session> createSessionFunc;

        public delegate void OnRecvHandler(INetSession userToken, short id, byte[] buffer);
        public delegate void OnDisConnectHandler(Session netToken);


        public OnRecvHandler onRecv;
        public OnDisConnectHandler onDisConnected;

        public abstract void Init(
            NetworkConfig config, 
            ILogger logger, 
            Assembly excuteAssembly, 
            string packetHandlerClassName,
            Func<Session> createSessionFunc);
        public abstract void Close();

        public void RecvComplete(object sender, SocketAsyncEventArgs args)
        {
            var session = args.UserToken as Session;
            try
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    if (onRecv == null)
                    {
                        logger?.Error("error not found OnReceive Func");
                        return;
                    }

                    session.ReceiveByteBuffer((id, bytes) =>
                    {
                        onRecv?.Invoke(session, id, bytes);
                        var packet = packetHandler?.GetPacket(id, bytes);
                        var handlerMethod = packetHandler.GetMethodInfo(id);
                        handlerMethod?.Invoke(null, new object[] { session, packet });

                    }, args);

                    session.Receive();
                }
                else
                {
                    throw new Exception($"Socker Close or SocketError err : {args.SocketError}");
                }
            }
            catch (Exception e)
            {
                logger?.Error($"RecvCompleted Error : {e.ToString()}");
                session.Disconnect();
                onDisConnected?.Invoke(session);
            }
        }

        public void SendComplete(object sender, SocketAsyncEventArgs args)
        {
            Session session = args.UserToken as Session;
            if (session == null)
            {
                Console.WriteLine("error SendComplete NetToken casting");
                return;
            }
            session.DequeueSend();
        }
    }
}
