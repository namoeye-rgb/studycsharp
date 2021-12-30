namespace NetLib
{
    public interface IPacketHeader
    {
        short UserPacketId { get; }
        ulong UniqueRequestId { get; }
        ulong AcknowledgedId { get; }
    }
}
