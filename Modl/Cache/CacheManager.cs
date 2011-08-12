using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Cache
{
    internal static class CacheManager
    {
        private static HashSet<Action> cacheList = new HashSet<Action>();

        internal static void RegisterClearMethod(Action clearMethod)
        {
            cacheList.Add(clearMethod);
        }

        internal static void Clear()
        {
            foreach (var cache in cacheList)
                cache.Invoke();
        }
    }
}
