using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityService
{
    public struct ESHandle
    {
        public static implicit operator ESHandle(ulong value) { return new ESHandle(value); }
        public static implicit operator ulong(ESHandle self) { return self.Key; }
        public static implicit operator ESHandle(GUID self) { return new ESHandle(self.Value); }
        public static readonly ESHandle Empty = new ESHandle(0);
        public readonly ulong Key;

        ESHandle(ulong key) { Key = key; }

        public static bool operator !=(ESHandle a, ESHandle b)
        {
            return a.Key != b.Key;
        }

        public static bool operator ==(ESHandle a, ESHandle b)
        {
            return a.Key == b.Key;
        }

        public static bool IsEmpty(ESHandle handle)
        {
            return Empty.Key == handle.Key;
        }

        public static ESHandle NewHandle()
        {
            return new ESHandle();
        }

        public override string ToString()
        {
            return Key.ToString();
        }

        public override int GetHashCode()
        {
            return (int)Key;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    public interface IHandled
    {
        ESHandle Handle { get; }
    }
}
