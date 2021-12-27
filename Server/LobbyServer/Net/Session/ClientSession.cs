using NetLib.Token;
using System;
using UtilLib;

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
