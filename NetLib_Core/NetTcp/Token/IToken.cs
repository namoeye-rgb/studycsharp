using System.Net.Sockets;

namespace NetLib.Token
{
    public interface IToken
    {
        Socket Socket { get; }
        SocketAsyncEventArgs ReceiveArgs { get; }
        SocketAsyncEventArgs SendArgs { get; }
    }

}
