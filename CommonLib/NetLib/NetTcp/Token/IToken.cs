using System.Net.Sockets;

namespace NetLib.Token
{
    public interface IToken
    {
        Socket Socket { get; }
        SocketAsyncEventArgs ReceiveArgs { get; }
        SocketAsyncEventArgs SendArgs { get; }
    }

    public interface IUserToken
    {
        void SendPacket<T>(T _packet) where T : PacketLib.PK_BASE;
        void ReceiveBuffer(IUserToken _userToken, byte[] _buffer);
        void Close();
    }
}
