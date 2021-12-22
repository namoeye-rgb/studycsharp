using System;
using System.Collections;
using System.Collections.Generic;

namespace EntityService
{
    public class ESObject : IEntity, IPropertyRead, IPropertyWrite
    {
        public ESObject() { }
        public ESObject(ESClass baseClass)
        {
            System.Diagnostics.Debug.Assert(baseClass != null, "'baseClass'가 null입니다. ", "Parameterless 생성자를 사용해야합니다");
            m_baseClass = baseClass;
        }

        private ESObject m_parent;
        private ESClass m_baseClass;
        private readonly Dictionary<string, ESProperty> m_properties = new Dictionary<string, ESProperty>();
        public string Idspace => m_baseClass.Idspace;
        public int ClassId => m_baseClass.ClassId;
        public string ClassName => m_baseClass.ClassName;
        public IDictionary InstanceProperties => m_properties;

        public void Clear()
        {
            m_baseClass = null;
            m_parent = null;
            m_properties.Clear();
        }
        public void Set(string key, ESProperty vaule)
        {
            m_properties[key] = vaule;
        }

        public void Set(string key, int vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public void Set(string key, float vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public void Set(string key, bool vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public void Set(string key, string vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public ESProperty GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"Key 값이 없다.(key : {key})");

            return m_properties[key];
        }

        public ESProperty GetPropertyOrEmpty(string key)
        {
            return m_properties.ContainsKey(key) ? m_properties[key] : ESProperty.Empty;
        }

        public bool GetBool(string key)
        {
            string boolString = GetString(key);
            bool result = false;
            if (!string.IsNullOrEmpty(boolString)) {
                bool.TryParse(boolString, out result);
            }

            return result;
        }

        public int GetInt(string key)
        {
            var prop = GetPropertyOrEmpty(key);
            if (prop.Type == ValueType.Invalid)
                return 0;
            if (prop.Type != ValueType.Number)
                return 0;
            return prop.GetInt();
        }

        public float GetFloat(string key)
        {
            var prop = GetPropertyOrEmpty(key);
            if (prop.Type == ValueType.Invalid)
                return 0;
            if (prop.Type != ValueType.Number)
                return 0;
            return prop.GetFloat();
        }

        public string GetString(string key)
        {
            var prop = GetPropertyOrEmpty(key);
            if (prop.Type == ValueType.Invalid)
                return string.Empty;
            return prop.GetString();
        }

        public bool ContainsKey(string key)
        {
            return m_properties.ContainsKey(key);
        }
    }
}
