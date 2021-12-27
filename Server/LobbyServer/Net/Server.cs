using GameDataLib;
using LobbyServer.DB;
using LobbyServer.Manager;
using NetLib;
using NetLib.Define;
using NetLib.Token;
using System;
using System.Threading;
using UtilLib;
using UtilLib.Log;

namespace LobbyServer.Net
{
    public partial class Server : Singleton<Server>
    {
        private TcpNetwork_Server server;

        //public PacketHandler packetHandler;


        public void Update()
        {
            while (true)
            {
                server.Update();
                Thread.Sleep(10);
            }
        }

        public void OnAccept(NetToken _acceptNetToken)
        {
            SessionMgr.Instance.CreateSession(_acceptNetToken);
            Log.Instance.Warn($"Client OnAccept");
        }
        public void OnReceive(IUserToken _userToken, ushort _packetId)
        {
           // packetHandler?.DispatchPacket(_packetId, _userToken, _packet);
        }

        public void SendPacket<T>(T _packet)
        {
            server.SendPacket(_packet);
        }

        public void OnDisConnect(NetToken _disconnectToken)
        {
            SessionMgr.Instance.OnDisconnect(_disconnectToken, out ClientSession session);

            session.Close();
        }
    }
}
