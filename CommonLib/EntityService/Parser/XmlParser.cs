using System;
using System.Xml;
using System.Xml.Linq;

namespace EntityService
{
    public class XmlParser
    {
        public ESTable ParseFromFile(string filename)
        {
            // XML
            var doc = new XmlDocument();
            try {
                doc.Load(filename);
            } catch (Exception e) {
                throw new Exception($"({typeof(XmlParser)}.ParseFromFile: {e.Message}");
            }
            return Parse(doc, filename);
        }

        private ESTable Parse(XmlDocument doc, string fileName)
        {
            // Begin parsing 구문 분석 시작
            var table = ParseIdspace(doc, fileName);

            if (!ParseSchema(table, doc)) {
                throw new Exception($"({typeof(XmlParser)}:스키마를 구문 분석하는 중에 오류가 발생했습니다. { fileName }");
            }

            if (!ParseData(table, doc)) {
                throw new Exception($"({typeof(XmlParser)}:데이터를 구문 분석하는 중에 오류가 발생했습니다. { fileName }");
            }

            return table;
        }

        private ESTable ParseIdspace(XmlDocument doc, string fileName)
        {
            if (doc.DocumentElement == null) {
                throw new Exception("잘못된 XML 파일.");
            }

            var tableId = doc.DocumentElement.GetAttribute(ESNames.Id);
            ESTable table = null;
            if (!ESTableManager.ContainsTable(tableId)) {
                table = new ESTable();
                table.Init(tableId, fileName);
            } else {
                table = ESTableManager.GetTable(tableId);
            }

            return table;
        }

        private bool ParseSchema(ESTable table, XmlDocument doc)
        {
            XDocument xdoc;
            using (var nodeReader = new XmlNodeReader(doc)) {
                nodeReader.MoveToContent();
                xdoc = XDocument.Load(nodeReader);
            }
            //var validators = xdoc.Element(EntityNames.IdSpace)?.Element(EntityNames.Schema.tagSchema)?.Elements(EntityNames.Schema.tagProperty);
            //var group = validators?.GroupBy(x => x.Attribute(EntityNames.Schema.attrName)?.Value);
            //var list = group?.ToList();
            //list?.ForEach(x => table.AddSchema(x.Key, new EntityPropertySchema(x.Select(e => e.ToPropertyValidator()).Where(v => v != null))));
            return true;
        }

        private bool ParseData(ESTable table, XmlDocument doc)
        {
            var idspace = doc.GetElementById(ESNames.IdSpace);
            var xmlData = doc.GetElementsByTagName(ESNames.Class);

            int index = 0;
            foreach (XmlNode xmlDatum in xmlData) {
                var data = new ESClass();

                XmlElement parent = (XmlElement)xmlDatum.ParentNode;
                if (parent != null && parent != idspace && parent.Name == ESNames.Category) {
                    data.Category = parent.GetAttribute(ESNames.Name);
                }

                if (xmlDatum.Attributes == null) {
                    continue;
                }

                int id = index++;
                var idAttribute = xmlDatum.Attributes[ESNames.ClassId];
                if (idAttribute != null) {
                    string idString = idAttribute.Value;
                    id = int.Parse(idString);
                    //xmlDatum.Attributes.Remove(idAttribute);
                }
                data.ClassId = id;

                // 존재하는 경우에만 이름이 고유해야합니다.
                var nameAttribute = xmlDatum.Attributes[ESNames.ClassName];
                if (nameAttribute != null && nameAttribute.Value.Length != 0) {
                    var name = nameAttribute.Value;
                    data.ClassName = name;
                    //xmlDatum.Attributes.Remove(nameAttribute);
                }

                ParseAttributes(data, xmlDatum.Attributes);

                table.AddClass(data);
            }

            return true;
        }

        private void ParseAttributes(ESClass data, XmlAttributeCollection attributes)
        {
            foreach (XmlAttribute attr in attributes) {
                if (attr.Name == ESNames.ClassId || attr.Name == ESNames.ClassName || attr.Value == "_") {
                    continue;
                }

                data.Set(attr.Name, attr.Value);
            }
        }
    }
}
