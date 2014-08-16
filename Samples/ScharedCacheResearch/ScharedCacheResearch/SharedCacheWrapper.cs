using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace ScharedCacheResearch
{
    public static class SharedCacheWrapper
    {
        public static T Get<T>(string key)
        {

            return IndexusDistributionCache.SharedCache.Get<T>(key);
        }

        public static void Add(string key, object value)
        {
            IndexusDistributionCache.SharedCache.Add(key, value);
        }
    }
}
