using System.Collections.Generic;

namespace Tamos.AbleOrigin
{
    internal class NullConfigProvider : ICentralConfigProvider
    {
        private readonly Dictionary<string, string> _store = new Dictionary<string, string>();
        
        public bool Set(string key, string value)
        {
            _store.Add(key, value);
            return true;
        }

        public string Get(string key)
        {
            string value;
            _store.TryGetValue(key, out value);
            return value;
        }

        public string GetAppSetting(string key)
        {
            return null;
        }
    }
}