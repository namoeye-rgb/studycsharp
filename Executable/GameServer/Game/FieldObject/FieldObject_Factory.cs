using EntityService;

namespace GameServer
{
    public sealed partial class FieldObject
    {
        public static FieldObject NewInsatnce(ESClass es, ESHandle handle)
        {
            var fo = new FieldObject(es, handle);
            return fo;
        }

    }
}
