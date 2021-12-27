using EntityService;
using System;

namespace GameServer
{
    public sealed partial class FieldMap : MessageTarget, IDisposable
    {
        private FieldMap(ESClass fieldMap, ESHandle handle) : base(fieldMap, handle)
        {
        }

        public void Dispose()
        {
            foreach (var component in m_components) {
                RemoveComponent(component.Key);
                component.Value.Dispose();
            }

            m_components.Clear();
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {

        }

        public void Update(float dt)
        {
            foreach (var component in m_components) {
                component.Value.OnUpdate(dt);
            }
        }

        public void LateUpdate(float dt)
        {
            foreach (var component in m_components) {
                component.Value.OnLateUpdate(dt);
            }
        }

        //public void OnFieldObjectSendMsg(ESHandle foHandle, MsgId msgId, MsgBody msg)
        //{
        //    if (ESHandle.IsEmpty(foHandle))
        //        return;

        //    var target = FMC_FieldObjectManager.GetFieldObject(foHandle);
        //    if (target == null)
        //        return;

        //    target.Receive(target, msgId, msg);
        //}

        //public void OnFieldObjectSendMsg<T>(ESHandle foHandle, MsgId msgId, MsgBody msg) where T : FieldObject
        //{
        //    if (ESHandle.IsEmpty(foHandle))
        //        return;

        //    var target = FieldObjectManager.GetFieldObject(foHandle);
        //    if (target == null)
        //        return;

        //    target.Receive(target, msgId, msg);
        //}

        public void OnReceiveMsg(IMessageTarget sender, MsgId msgId, MsgBody msg)
        {
            foreach (var component in m_components.Values) {
                component.OnPreReceiveMsg(sender, msgId, msg);
            }

            foreach (var component in m_components.Values) {
                component.OnReceiveMsg(sender, msgId, msg);
            }
        }
    }
}
