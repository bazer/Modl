using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Structure;

namespace Modl
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheAttribute : Attribute
    {
        public int CacheTimeout { get; private set; }
        public CacheLevel CacheLevel { get; private set; }

        public CacheAttribute(CacheLevel cacheLevel)
        {
            CacheTimeout = Settings.GlobalSettings.CacheTimeout;
            CacheLevel = cacheLevel;
        }

        public CacheAttribute(CacheLevel cacheLevel, int minutesToTimeout)
        {
            CacheTimeout = minutesToTimeout;
            CacheLevel = cacheLevel;

        }

        public CacheAttribute(CacheLevel cacheLevel, CacheTimeout timeout)
        {
            CacheTimeout = (int)timeout;
            CacheLevel = cacheLevel;
        }
    }
}
