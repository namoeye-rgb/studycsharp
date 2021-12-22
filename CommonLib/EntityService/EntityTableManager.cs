using System;
using System.Collections.Generic;

namespace EntityService
{
    public static class ESTableManager
    {
        private static Dictionary<string, ESTable> m_tables = new Dictionary<string, ESTable>();
        private static readonly XmlParser m_loader = new XmlParser();

        public static bool Save(string idSpace)
        {
            if (!m_tables.TryGetValue(idSpace, out var table)) {
                return false;
            }
            return table.Save();
        }

        public static bool Load(string fileName)
        {
            var loadTable = m_loader.ParseFromFile(fileName);
            if (loadTable == null) {
                throw new Exception($"({typeof(XmlParser)} 구문 분석 - 파일 이름 '{fileName}' 없다.");
            }

            m_tables.Add(loadTable.Idspace, loadTable);
            return true;

        }

        public static bool ContainsTable(string idSpace)
        {
            return m_tables.ContainsKey(idSpace);
        }

        public static ESTable GetTable(string idSpace)
        {
            if (!m_tables.TryGetValue(idSpace, out var table)) {
                throw new NullReferenceException($"{idSpace} 테이블은 없습니다.");
            }

            return table;
        }

        public static ESClass GetClass(string idSpace, int classId)
        {
            if (!m_tables.ContainsKey(idSpace)) {
                throw new NullReferenceException($"{idSpace} 테이블은 없습니다.");
            }

            if (!m_tables[idSpace].ContainsKey(classId)) {
                throw new NullReferenceException($"{idSpace}에 ClassId:{classId}는 존재하지 않습니다. ");
            }

            return m_tables[idSpace].GetClass(classId);
        }

        public static ESClass GetClass(string idSpace, string className)
        {
            if (!m_tables.ContainsKey(idSpace)) {
                throw new NullReferenceException($"{idSpace} 테이블은 없습니다.");
            }

            if (!m_tables[idSpace].ContainsKey(className)) {
                throw new NullReferenceException($"{idSpace}에 ClassId:{className}는 존재하지 않습니다. ");
            }

            return m_tables[idSpace].GetClass(className);
        }

        public static void Clear()
        {
            m_tables.Clear();
        }
    }
}