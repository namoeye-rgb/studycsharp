namespace EntityService
{
    public interface IMessageTarget : IHandled, IEntity
    {
        ESObject Info { get; }

        void SendMsg(IMessageTarget target, MsgId msgId, MsgBody msg);
        void Receive(IMessageTarget sender, MsgId msgId, MsgBody msg);
    }

    public interface IMessageHandler
    {
        bool OnPreSend(IMessageTarget target, MsgId msgId, MsgBody msg);
        bool OnPreReceive(IMessageTarget sender, MsgId msgId, MsgBody msg);
        void OnReceive(IMessageTarget sender, MsgId msgId, MsgBody msg);
    }
}
