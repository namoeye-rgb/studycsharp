
namespace EntityService
{
    public struct MsgId
    {
        public readonly uint Key;
        private MsgId(uint key) { Key = key; }
        public static implicit operator MsgId(uint value) { return new MsgId(value); }
        public static implicit operator uint(MsgId self) { return self.Key; }
    }

    public abstract class MsgBody
    {
    }
}
