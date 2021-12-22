using EntityService;
using GameCommon.Contracts.Message;
using System;
using System.Collections.Generic;

namespace GameServer
{
    public sealed partial class NonFieldObject : MessageTarget
    {
        private readonly Dictionary<Type, FOComponent> m_components = new Dictionary<Type, FOComponent>();
        public T AddComponent<T>() where T : FOComponent, new()
        {
            m_components.TryGetValue(typeof(T), out var component);
            if (component != null) {
                throw new Exception("이미 같은 타입의 컴포넌트가 존재함.");
            }

            var com = FOComponent.Create<T>();
            com.SendMsg(this, MSGID.CompAdded, new CompAdded() {
                Owner = this,
            });
            m_components.Add(typeof(T), com);
            return com;
        }

        public void RemoveComponent<T>() where T : FOComponent, new()
        {
            m_components.TryGetValue(typeof(T), out var component);
            if (component == null) {
                throw new Exception("컴포넌트가 없음.");
            }

            component.SendMsg(this, MSGID.CompRemoved, null);
            component.Dispose();
            m_components.Remove(typeof(T));
        }

        public void RemoveComponent(Type key)
        {
            if (m_components.TryGetValue(key, out var component)) {
                m_components.Remove(key);
                component.SendMsg(this, MSGID.CompRemoved, null);
                component.Dispose();
            }
        }

        internal T GetComponent<T>() where T : FOComponent
        {

            m_components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        public bool HasComponent<T>() where T : FOComponent
        {
            foreach (var component in m_components.Values) {
                if (component is T == false) {
                    continue;
                }
            }
            return true;
        }

    }
}
