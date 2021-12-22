using EntityService;
using System;

namespace GameServer
{
    public abstract class FMComponent : IDisposable
    {
        protected internal FMComponent() { }
        public FieldMap Owner { get; set; }

        public virtual void OnPreReceiveMsg(IMessageTarget sender, MsgId msgId, MsgBody msgBody) { }
        public virtual void OnReceiveMsg(IMessageTarget sender, MsgId msgId, MsgBody msgBody) { }

        public void Dispose()
        {
            OnFinalize();
            Owner = null;
        }

        public virtual void OnCreated() { }
        public virtual void OnInitialize() { }
        public virtual void OnUpdate(float dt) { }
        public virtual void OnLateUpdate(float dt) { }
        protected virtual void OnFinalize() { }

        public static T NewInstance<T>(FieldMap owner) where T : FMComponent, new()
        {
            var component = new T {
                Owner = owner,
            };
            return component;
        }

        // 메시지 처리합시다
    }
}
