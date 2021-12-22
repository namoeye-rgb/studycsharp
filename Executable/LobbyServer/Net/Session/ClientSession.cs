using EntityService;
using NetLib.Token;
using PacketLib;
using System;

namespace LobbyServer.Net
{
    public class ClientSession : IUserToken
    {
        private readonly NetToken netToken;

        public GUID guid;

        public string UserUID { get; private set; }

        public ClientSession(NetToken _netToken, GUID _guid)
        {
            netToken = _netToken;
            guid = _guid;
        }

        public void SendPacket<T>(T _packet) where T : PK_BASE
        {
            var packetBuffer = Serializer.Serialize_Server(_packet);
            netToken.SendPacket(packetBuffer);
        }

        public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        {
            throw new NotImplementedException();
        }

        public void SetUserUID(string _userUID)
        {
            UserUID = _userUID;
        }

        public void Close()
        {
            netToken.Close();
        }
    }
}
