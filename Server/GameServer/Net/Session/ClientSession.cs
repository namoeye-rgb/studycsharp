using NetLib;
using NetLib.Peer;
using NetLib.Token;
using PacketLib;

namespace GameServer.Net.Session
{
    /// <summary> UnityClient와 통신함 </summary>
    public class ClientSession
    {
        RakPeerSession rakPeer;
        public ulong UserUID { get; set; }

        public ulong Guid {
            get {
                return rakPeer.Guid;
            }
        }

        public void Initialize(ulong _guid, RakServer _rakServer)
        {
            rakPeer = new RakPeerSession();
            rakPeer.Initialize(_guid);
            rakPeer.SetRakServer(_rakServer);
        }

        public void Close()
        {
            rakPeer.RakServer.CloseConnection(rakPeer.RakNetGuid, true);
        }

        public void SendPacket<T>(T _packet) where T : PK_BASE
        {
            var buffer = Serializer.Serialize_RakServer(_packet);
            rakPeer.Send(buffer);
        }

        public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        {
            throw new System.NotImplementedException();
        }
    }
}
