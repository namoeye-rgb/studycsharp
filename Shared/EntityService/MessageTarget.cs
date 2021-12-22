using System;

namespace EntityService
{
    public abstract class MessageTarget : IMessageTarget
    {
        protected MessageTarget()
        {
        }

        protected MessageTarget(ESClass es, ESHandle handle)
        {
            Info = new ESObject(es);
            Handle = handle;
        }

        public ESObject Info { get; private set; }
        public ESHandle Handle { get; private set; }
        public string Idspace => Info.Idspace;
        public int ClassId => Info.ClassId;
        public string ClassName => Info.ClassName;
        private IMessageHandler m_instanceMessageHandler;

        public void ClearMsg()
        {
            m_instanceMessageHandler = null;
        }

        public void SetMessageHandler(IMessageHandler handler)
        {
            m_instanceMessageHandler = handler;
        }

        public void SendMsg(IMessageTarget target, MsgId msgId, MsgBody msgBody)
        {
            if (m_instanceMessageHandler?.OnPreSend(target, msgId, msgBody) ?? false)
                return;

            target?.Receive(this, msgId, msgBody);
        }

        public void Receive(IMessageTarget sender, MsgId msgId, MsgBody msgBody)
        {
            m_instanceMessageHandler.OnReceive(this, msgId, msgBody);
        }
    }
}
