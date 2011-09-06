/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheAttribute : Attribute
    {
        public int CacheTimeout { get; private set; }
        public CacheLevel CacheLevel { get; private set; }

        public CacheAttribute(CacheLevel cacheLevel)
        {
            CacheTimeout = Config.DefaultCacheTimeout;
            CacheLevel = cacheLevel;
        }

        public CacheAttribute(CacheLevel cacheLevel, int minutesToTimeout)
        {
            CacheTimeout = minutesToTimeout;
            CacheLevel = cacheLevel;

        }

        public CacheAttribute(CacheLevel cacheLevel, Timeout timeout)
        {
            CacheTimeout = (int)timeout;
            CacheLevel = cacheLevel;
        }
    }
}
