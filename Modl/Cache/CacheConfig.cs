using System;
using System.Collections.Generic;
using System.Threading;
using Modl.Structure;

namespace Modl.Cache
{
    public class CacheConfig
    {
        public static CacheLevel DefaultCacheLevel { get; set; }
        public static int DefaultCacheTimeout { get; set; }

        private static HashSet<Action> cacheList = new HashSet<Action>();

        public static void SetDefaultCacheTimeout(CacheTimeout timeout)
        {
            DefaultCacheTimeout = (int)timeout;
        }

        internal static void RegisterClearMethod(Action clearMethod)
        {
            cacheList.Add(clearMethod);
        }

        public static void Clear()
        {
            foreach (var cache in cacheList)
                cache.Invoke();
        }
    }
}
