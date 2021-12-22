using NetLib;
using NetLib.Token;
using PacketLib;
using PacketLib.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using UtilLib;
using UtilLib.Log;

namespace GameServer.Net
{
    public partial class S2SClient : Singleton<S2SClient>, IUserToken
    {
        public NetworkHandler TcpClient { get; private set; }

        private int packetSeqNumber;
        public ushort S2SClientNumber = 0;
        private string ip = string.Empty;
        private int port = 0;

        private PacketHandler packetHandler;
        private Dictionary<int, Action<PK_BASE>> waitResponseCall = new Dictionary<int, Action<PK_BASE>>();

        public void Init()
        {
            TcpClient = new NetworkHandler(Log.Instance, NETCLIENT_TYPE.S2S);
            TcpClient.OnConnect = ConnectNetToken;
            TcpClient.OnReceive = ReceiveBuffer;
            TcpClient.OnDisConnect = DisConnect;

            TcpClient.Init_Client();

            packetHandler = new PacketHandler();
            packetHandler.SetHandler(typeof(Packet.PacketHandler_S2S), "PacketReceive", (x, c, m) => {
                var pksId = PacketList.Get(x[0].ParameterType);
                if (pksId == 0) {
                    return;
                }

                c.Add(pksId, m);
            });

            PacketParser.InitParseMethod();
        }

        public void Start_Client(string _ip, int _port)
        {
            ip = _ip;
            port = _port;
            TcpClient.Start_Client(ip, port);
        }

        public void DisConnect(NetToken _netToken)
        {
            TcpClient.Start_Client(ip, port);
        }

        private void ConnectNetToken(NetToken _netToken)
        {
            _netToken.SetUserToken(this);

            var s2sclient_packet = new PK_S2S_REQ_CONNECT();
            s2sclient_packet.ServerType = (int)Server.Instance.serverInfo.Config.ServerType;

            SendPacket(s2sclient_packet);
        }
        public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        {
            Deserializer.Deserialize_Server_S2S(_buffer, out PK_BASE packet, out ushort packetId, out int packetNumbering);

            if (waitResponseCall.ContainsKey(packetNumbering)) {
                waitResponseCall[packetNumbering](packet);
                waitResponseCall.Remove(packetNumbering);
                return;
            }

            packetHandler.DispatchPacket(packetId, packet);
        }

        public void SendPacket<T>(T _packet) where T : PK_BASE
        {
            var packetBuffer = Serializer.Serialize_Server_S2S(_packet, 0);
            TcpClient.SendPacket(packetBuffer);
        }

        public void SendPacket<T>(T _packet, Action<PK_BASE> _responseMethod) where T : PK_BASE
        {
            Interlocked.Increment(ref packetSeqNumber);
            var packetBuffer = Serializer.Serialize_Server_S2S(_packet, packetSeqNumber);

            waitResponseCall.Add(packetSeqNumber, _responseMethod);

            TcpClient.SendPacket(packetBuffer);
        }

        public void Close()
        {
            TcpClient.Close();
        }

        public bool IsAvailable()
        {
            return TcpClient.GetSocketState() == SOCKET_STATE.SUCCESS;
        }
    }
}
