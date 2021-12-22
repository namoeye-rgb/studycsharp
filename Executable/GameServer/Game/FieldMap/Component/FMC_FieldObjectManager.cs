using EntityService;
using System.Collections.Generic;

namespace GameServer
{
    public class FMC_FieldObjectManager : FMComponent
    {
        /// <summary> 필드맵에 있는 오브젝트들 리스트 </summary>
        internal readonly Dictionary<ESHandle, FieldObject> m_fieldObjects = new Dictionary<ESHandle, FieldObject>();
        public int FieldObjectCount => m_fieldObjects.Count;

        /// <summary> 지정한 필드오브젝트가 있으면 리턴, 없으면 Null </summary>
        public FieldObject GetFieldObject(ESHandle handle)
        {
            if (m_fieldObjects.TryGetValue(handle, out var fo)) {
                return fo;
            }

            return null;
        }

        /// <summary> 필드오브젝트를 추가합니다. </summary>
        public FieldObject AddFieldObject(ESClass es)
        {
            var fo = FieldObject.NewInsatnce(es, GUIDGen.CreateGUID());
            m_fieldObjects.Add(fo.Handle, fo);
            return fo;
        }

        /// <summary> 지정한 필드오브젝트 삭제합니다. </summary>
        public void RemoveFieldObject(ESHandle handle)
        {
            if (m_fieldObjects.TryGetValue(handle, out var fo)) {
                fo.Dispose();
                m_fieldObjects.Remove(handle);
            }
        }

        /// <summary> 모든 필드오브젝트를 제거합니다. </summary>
        public void ClearFieldObject()
        {
            foreach (var fieldObject in m_fieldObjects) {
                fieldObject.Value.Dispose();
            }
            m_fieldObjects.Clear();
        }

        /// <summary> 모든 필드오브젝트를 리턴함 </summary>
        public IEnumerable<FieldObject> GetFieldObjects()
        {
            return m_fieldObjects.Values;
        }
    }
}
