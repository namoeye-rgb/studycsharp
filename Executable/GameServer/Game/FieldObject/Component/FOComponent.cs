using EntityService;
using System;

namespace GameServer
{
    public abstract class FOComponent : IDisposable
    {
        protected FOComponent() { }
        public FieldObject Owner { get; protected set; }

        public void SendMsg(IMessageTarget sender, MsgId msgId, MsgBody msgBody)
        {
            OnReceiveMsg(sender, msgId, msgBody);
        }

        protected virtual void OnReceiveMsg(IMessageTarget sender, MsgId msgId, MsgBody msgBody) { }

        public void Dispose()
        {
            OnDispose();
            Owner = null;
        }

        public virtual void Initialize() { }
        public virtual void Update(float dt) { }
        public virtual void LateUpdate(float dt) { }
        protected virtual void OnDispose() { }

        public static T Create<T>() where T : FOComponent, new()
        {
            var component = new T();
            return component;
        }

        // 메시지 처리합시다
    }
}
