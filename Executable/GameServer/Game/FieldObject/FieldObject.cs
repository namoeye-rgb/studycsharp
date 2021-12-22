using EntityService;
using System;

namespace GameServer
{
    public sealed partial class FieldObject : MessageTarget, IDisposable
    {
        private FieldObject(ESClass es, ESHandle handle) : base(es, handle)
        {
        }

        public void Initialize()
        {
            AddComponent<FOC_FiniteStateMachine>();
            AddComponent<FOC_BuffInventory>();
            AddComponent<FOC_Synchronization>();
        }

        public void Update(float dt)
        {
            foreach (var component in m_components) {
                component.Value.Update(dt);
            }
        }

        public void LateUpdate(float dt)
        {
            foreach (var component in m_components) {
                component.Value.LateUpdate(dt);
            }
        }

        public void Dispose()
        {
            foreach (var component in m_components) {
                RemoveComponent(component.Key);
            }

            m_components.Clear();
        }
    }
}