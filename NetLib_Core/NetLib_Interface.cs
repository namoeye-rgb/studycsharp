using Google.Protobuf;

namespace NetLib
{
    public interface IPacketHeader
    {
        short UserPacketId { get; }
        ulong UniqueRequestId { get; }
        ulong AcknowledgedId { get; }
    }

    public interface INetSession
    {
        void SendPacket<T>(T sendPacket) where T : IMessage;
        void Close();
        void Disconnect();
    }

    public interface IUser
    {
        string GetUserID();
    }

}
