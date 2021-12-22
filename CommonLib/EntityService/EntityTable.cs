using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EntityService
{
    public class ESTable
    {
        public string FileName { get; private set; }
        public string Idspace { get; private set; }
        public int Count => classById.Count;

        private readonly Dictionary<int, ESClass> classById;
        private readonly Dictionary<string, ESClass> classByName;
        private readonly Dictionary<string, List<ESClass>> classListByCategory;

        public ESTable()
        {
            classById = new Dictionary<int, ESClass>();
            classByName = new Dictionary<string, ESClass>();
            classListByCategory = new Dictionary<string, List<ESClass>>();
        }

        public bool Save()
        {
            if (string.IsNullOrEmpty(Idspace)) {
                throw new NullReferenceException($"{ESNames.IdSpace}({Idspace}) is null");
            }

            if (string.IsNullOrEmpty(FileName)) {
                throw new NullReferenceException($"{ESNames.FileName} is null");
            }

            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            var rootElement = doc.CreateElement(ESNames.IdSpace);
            rootElement.SetAttribute(ESNames.Id, Idspace);

            classById.OrderBy(i => i.Key).ToList().ForEach(pair => {
                var data = pair.Value;
                var element = doc.CreateElement(ESNames.Class);

                // 아이디, 이름
                element.SetAttribute(ESNames.ClassId, data.ClassId.ToString());
                element.SetAttribute(ESNames.ClassName, data.ClassName);

                // 데이터
                data.SaveProperty(element);

                // 카테고리
                if (string.IsNullOrEmpty(data.Category)) {
                    rootElement.AppendChild(element);
                } else {
                    var categoryList = rootElement.SelectNodes(ESNames.Category);
                    XmlElement parent = null;
                    if (categoryList != null) {
                        foreach (XmlNode n in categoryList) {
                            var elem = (XmlElement)n;
                            if (elem.Name == ESNames.Category && elem.GetAttribute(ESNames.Name) == data.Category) {
                                parent = elem;
                            }
                        }
                    }
                    if (parent != null) {
                        parent.AppendChild(element);
                    } else {
                        var cateElem = (XmlElement)rootElement.AppendChild(doc.CreateElement(ESNames.Category));
                        cateElem.SetAttribute(ESNames.Name, data.Category);
                        cateElem.AppendChild(element);
                    }
                }
            });
            doc.AppendChild(rootElement);
            //doc.Save(FileName);

            //if (PropertySchemata?.Any() ?? false) {
            //    var pse = PropertySchemata.ToXElement(Names.Schema.tagSchema);
            //    using (XmlReader xmlReader = pse.CreateReader()) {
            //        var helperDoc = new XmlDocument();
            //        helperDoc.Load(xmlReader);
            //        var child = doc.DocumentElement?.FirstChild;
            //        doc.DocumentElement?.InsertBefore(doc.ImportNode(helperDoc.FirstChild, true), child);
            //    }
            //}

            try {
                var xmlWriter = XmlWriter.Create(FileName, new XmlWriterSettings() {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    IndentChars = "\t",
                    ConformanceLevel = ConformanceLevel.Auto,
                    NewLineOnAttributes = true,
                    NewLineHandling = NewLineHandling.None,
                    CloseOutput = true
                });
                doc.WriteTo(xmlWriter);
                xmlWriter.Flush();
                xmlWriter.Close();
            } catch (Exception ex) {
                throw new NullReferenceException($"Idspace is null {ex.Message}");
            }

            return true;
        }

        public void Init(string idSpace, string fileName)
        {
            Idspace = idSpace;
            FileName = fileName;
        }

        public bool ContainsKey(string key)
        {
            return classByName.ContainsKey(key);
        }

        public bool ContainsKey(int key)
        {
            return classById.ContainsKey(key);
        }

        public bool AddClass(ESClass entityClass)
        {
            if (entityClass.Idspace == null
                || entityClass.ClassName == null
                || entityClass.ClassId == -1) {
                throw new NullReferenceException(
                    $"IdSpace:{entityClass.Idspace}, ClassName:{entityClass.ClassName}, ClassId:{entityClass.ClassId}");
            }

            if (classById.ContainsKey(entityClass.ClassId)) {
                throw new NullReferenceException($"{Idspace} 중복 ClassId {entityClass.ClassId}");
            }

            if (classByName.ContainsKey(entityClass.ClassName)) {
                throw new NullReferenceException($"{Idspace} 중복 ClassName {entityClass.ClassName}");
            }

            // 특수 처리해줄수 있음.

            if (!classListByCategory.ContainsKey(entityClass.Idspace)) {
                List<ESClass> list = new List<ESClass>();
                list.Add(entityClass);
                classListByCategory.Add(entityClass.Idspace, list);
            } else {
                classListByCategory[entityClass.Idspace].Add(entityClass);
            }

            classById.Add(entityClass.ClassId, entityClass);
            classByName.Add(entityClass.ClassName, entityClass);

            return true;
        }

        public void AddSchema(string name, ESPropertyValidator validator)
        {
        }

        public bool RemoveClass(ESClass entityClass)
        {
            if (classById.ContainsKey(entityClass.ClassId)) {
                classById.Remove(entityClass.ClassId);
            }

            if (classByName.ContainsKey(entityClass.ClassName)) {
                classByName.Remove(entityClass.ClassName);
            }

            if (classListByCategory.ContainsKey(entityClass.Idspace)) {
                classListByCategory[entityClass.Idspace].Remove(entityClass);
            }

            return true;
        }

        public void RemoveSchema(string name, ESPropertyValidator validator)
        {
        }

        public Dictionary<string, List<ESClass>> GetCategorys()
        {
            if (classListByCategory.Count == 0) {
                throw new NullReferenceException("Category Count == 0");
            }

            return classListByCategory;
        }

        public List<ESClass> GetCategory(string key)
        {
            if (!classListByCategory.ContainsKey(key)) {
                throw new NullReferenceException($"classListByCategory Not Key({key})!");
            }

            return classListByCategory[key];
        }

        public ESClass GetClass(string key)
        {
            if (!classByName.ContainsKey(key)) {
                throw new NullReferenceException($"classByName Not Key({key})!");
            }

            return classByName[key];
        }

        public ESClass GetClass(int key)
        {
            if (!classById.ContainsKey(key)) {
                throw new NullReferenceException($"classById Not Key({key})!");
            }

            return classById[key];
        }
    }
}