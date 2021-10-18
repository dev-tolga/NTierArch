using System.Collections.Generic;

namespace NTier.Core.Caching
{
    public interface IRedisCacheService
    { 
        T Get<T>(string key);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        bool Remove(string key);
        void RemoveByPattern(string pattern);
        void Clear();
        void SetAll<T>(IDictionary<string, T> values);

    }
}
