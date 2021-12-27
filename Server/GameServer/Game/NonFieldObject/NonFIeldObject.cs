using EntityService;
using System;

namespace GameServer
{
    public sealed partial class NonFieldObject : IDisposable
    {
        private NonFieldObject(ESClass es, ESHandle handle) : base(es, handle)
        {
        }

        public void Initialize()
        {
        }

        public void Update()
        {
        }

        public void LateUpdate()
        {
        }

        public void Dispose()
        {

        }
    }
}
