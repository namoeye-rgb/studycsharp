using NetLib.Define;
using NetLib.Token;
using PacketLib;
using System;

namespace SyncServer.Net.Session
{
    public class ClientSession : IUserToken
    {
        public NetToken NetToken { get; private set; }

        public ushort SessionUID { get; }

        public SERVER_TYPE ServerType { get; set; }

        public int PacketSeqNumber { get; set; }

        public ClientSession(NetToken _netToken, ushort _id)
        {
            NetToken = _netToken;
            SessionUID = _id;
        }

        public void SendPacket<T>(T _packet) where T : PK_BASE
        {
            var packetBuffer = Serializer.Serialize_Server_S2S(_packet, PacketSeqNumber);
            NetToken.SendPacket(packetBuffer);
        }

        public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            NetToken.Close();
        }
    }
}
