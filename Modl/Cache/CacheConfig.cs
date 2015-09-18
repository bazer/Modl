﻿/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

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
