using System;

namespace Tamos.AbleOrigin
{
    internal class DistributedLock : IDisposable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Dispose()
        {
            DistributedService.Provider.Unlock(Name, Value);
        }
    }
}