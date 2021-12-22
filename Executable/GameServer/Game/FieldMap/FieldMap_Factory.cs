using EntityService;

namespace GameServer
{
    public sealed partial class FieldMap
    {
        public static FieldMap NewInsatnce(ESClass es, ESHandle handle)
        {
            var fo = new FieldMap(es, handle);
            return fo;
        }

    }
}
