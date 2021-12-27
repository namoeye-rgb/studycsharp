namespace NetLib
{
    public interface INetLogger
    {
        void Debug(string logMessage, params object[] logParams);

        void Info(string logMessage, params object[] logParams);

        void Warn(string logMessage, params object[] logParams);

        void Error(string logMessage, params object[] logParams);

        void Fatal(string logMessage, params object[] logParams);

        void Exception(string logMessage, System.Exception exceptionData);
    }

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
