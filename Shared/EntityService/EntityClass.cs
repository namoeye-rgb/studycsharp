using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace EntityService {
    public class ESClass : IEntity, IPropertyRead, IPropertyWrite {
        public ESTable Table { get; internal set; }
        public int ClassId { get; internal set; }
        public string ClassName { get; internal set; }
        public string Category { get; internal set; }
        public string Idspace => Table.Idspace;

        private readonly Dictionary<string, ESProperty> m_properties;
        public ESClass()
        {
            ClassId = -1;
            ClassName = null;
            Category = null;

            m_properties = new Dictionary<string, ESProperty>();
        }

        public ESClass(ESTable table, int classId, string className, string category)
        {
            Table = table;
            ClassId = classId;
            ClassName = className;
            Category = category;

            m_properties = new Dictionary<string, ESProperty>();
        }

        public bool ContainsKey(string key)
        {
            return m_properties.ContainsKey(key);
        }

        public void SaveProperty(XmlElement element)
        {
            // 정렬방법
            string[] propertyPriority = null;
            if (propertyPriority != null) {
                // priority attributes
                foreach (string key in propertyPriority) {
                    if (ContainsKey(key)) {
                        var property = GetPropertyOrEmpty(key);

                        string attrvalue;
                        if (property.Type == ValueType.Number) {
                            attrvalue = property.GetFloat().ToString("0.0#####");
                        } else {
                            attrvalue = property.GetString();
                        }
                        element.SetAttribute(key, attrvalue);
                    }
                }

                // other attributes
                foreach (var property in m_properties) {
                    if (!propertyPriority.Contains(property.Key)) {
                        string attrvalue;
                        if (property.Value.Type == ValueType.Number) {
                            attrvalue = property.Value.GetFloat().ToString("0.0#####");
                        } else {
                            attrvalue = property.Value.GetString();
                        }
                        element.SetAttribute(property.Key, attrvalue);
                    }
                }
            } else {

                var list = m_properties.Keys.ToList();
                list.Sort();
                foreach (string key in list) {
                    string attrvalue;
                    if (m_properties[key].Type == ValueType.Number) {
                        attrvalue = m_properties[key].GetFloat().ToString("0.0#####");
                    } else {
                        attrvalue = m_properties[key].GetString();
                    }
                    element.SetAttribute(key, attrvalue);
                }
            }
        }
        public void Set(string key, ESProperty property)
        {
            if (m_properties.ContainsKey(key))
                new Exception("클래스는 프로퍼티를 재정의 할 수 없습니다.");

            m_properties.Add(key, property);
        }

        public void Set(string key, bool vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public void Set(string key, int vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public void Set(string key, float vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public void Set(string key, string vaule)
        {
            Set(key, new ESProperty(vaule));
        }

        public ESProperty GetProperty(string key)
        {
            if (!m_properties.ContainsKey(key)) {
                new Exception($"key({key})에 해당하는 프로퍼티가 없습니다.");
            }

            return m_properties[key];
        }

        public ESProperty GetPropertyOrEmpty(string key)
        {
            if (m_properties.ContainsKey(key)) {
                return m_properties[key];
            }

            return ESProperty.Empty;
        }
        internal bool TryGetProperty(string key, out ESProperty result)
        {
            if (m_properties.TryGetValue(key, out result))
                return true;

            //if (m_extendParent != null) {
            //    foreach (var extend in m_extendParent) {
            //        if (extend.TryGetProperty(key, out result)) {
            //            return true;
            //        }
            //    }
            //}

            return false;
        }

        public bool GetBool(string key)
        {
            string boolString = GetString(key);
            bool b = false;
            if (!string.IsNullOrEmpty(boolString)) {
                bool.TryParse(boolString, out b);
            }
            return b;
        }

        public int GetInt(string key)
        {
            return m_properties[key].GetInt();
        }

        public float GetFloat(string key)
        {
            return m_properties[key].GetFloat();
        }

        public string GetString(string key)
        {
            return m_properties[key].GetString();
        }
    }
}
