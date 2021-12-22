using System;
using System.Collections.Generic;

namespace GameServer
{
    public sealed partial class FieldMap
    {
        private readonly Dictionary<Type, FMComponent> m_components = new Dictionary<Type, FMComponent>();

        public T AddComponent<T>() where T : FMComponent, new()
        {
            m_components.TryGetValue(typeof(T), out var component);
            if (component != null) {
                throw new Exception("이미 같은 타입의 컴포넌트가 존재함.");
            }

            var com = FMComponent.NewInstance<T>(this);
            m_components.Add(typeof(T), com);
            //component.SendMsg(this, MSGID.CompAdded, null);
            return com;
        }

        public void RemoveComponent<T>() where T : FMComponent, new()
        {
            if (!m_components.TryGetValue(typeof(T), out var component)) {
                return;
            }

            m_components.Remove(typeof(T));
            //component.SendMsg(this, MSGID.CompRemoved, null);
            component.Dispose();
        }

        public void RemoveComponent(Type key)
        {
            if (!m_components.TryGetValue(key, out var component)) {
                return;
            }

            m_components.Remove(key);
            //component.SendMsg(this, MSGID.CompRemoved, null);
            component.Dispose();
        }

        public T GetComponent<T>() where T : FMComponent
        {
            m_components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        public bool HasComponent<T>() where T : FMComponent
        {
            return m_components.ContainsKey(typeof(T));
        }
    }
}
