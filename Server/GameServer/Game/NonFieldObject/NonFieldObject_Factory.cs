using EntityService;

namespace GameServer
{
    public sealed partial class NonFieldObject : MessageTarget
    {
        public static NonFieldObject NewInsatnce(ESClass es, ESHandle handle)
        {
            var fo = new NonFieldObject(es, handle);
            return fo;
        }
    }
}
