using NetLib;
using NetLib.Token;
using UtilLib.Log;

namespace LobbyServer.Net
{
    public class TcpNetwork_Server : IUserToken
    {
        public NetworkHandler TcpServer { get; }

        public delegate void AcceptClient_CallBack(NetToken _netToken);
        public delegate void ReceiveBufferComplete(IUserToken _userToken, ushort _packetId);
        public delegate void DisConnect_CallBack(NetToken _netToken);

        public AcceptClient_CallBack OnAccept;
        public ReceiveBufferComplete OnReceiveComplete;
        public DisConnect_CallBack OnDisConnect;

        public TcpNetwork_Server()
        {
            TcpServer = new NetworkHandler(Log.Instance, NETCLIENT_TYPE.CS);
            TcpServer.OnAccept = AcceptNetToken;
            TcpServer.OnReceive = ReceiveBuffer;
            TcpServer.OnDisConnect = DisConnect;
        }
        public void AcceptNetToken(NetToken _netToken)
        {
            OnAccept?.Invoke(_netToken);
        }
        public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        {
            //Deserializer.Deserialize_Server(_buffer, out PK_BASE packet, out ushort packetId);

           // OnReceiveComplete?.Invoke(_userToken, packetId, packet);
        }

        public void ServerStart(int _port, int _maxConnection)
        {
            TcpServer.Start_Server(_port, _maxConnection);
        }

        public void Update()
        {
            TcpServer.Update(0);
        }

        public void SendPacket<T>(T _packet)
        {
            //var packetBuffer = Serializer.Serialize_Server(_packet);
            //TcpServer.SendPacket(packetBuffer);
        }

        public void DisConnect(NetToken _netToken)
        {
            OnDisConnect?.Invoke(_netToken);
            _netToken.Close();
        }

        public void Close()
        {
            TcpServer.Close();
        }
    }
}
