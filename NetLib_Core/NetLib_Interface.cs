namespace NetLib
{
    public interface ISession
    {
        void Initialize(ulong _guid);
    }

    public interface IPacketHeader
    {
        short UserPacketId { get; }
        ulong UniqueRequestId { get; }
        ulong AcknowledgedId { get; }
    }
}
