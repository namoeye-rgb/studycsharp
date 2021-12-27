using System;
using System.Reflection;

namespace UtilLib
{
    public class Singleton<T> where T : class
    {
        private static readonly Lazy<T> _Lazy = new Lazy<T>(CreateInstance);
        public static T Instance {
            get {
                return _Lazy.Value;
            }
        }

        private static T CreateInstance()
        {
            return Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, null, null) as T;
        }
    }
}
