/*
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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Configuration;

using Modl.Cache;
using Modl.Structure;

namespace Modl.Structure
{
    public enum CacheLevel
    {
        On,
        Off,
        All
    }

    public enum CacheTimeout
    {
        Never = 0,
        TenMinutes = 10,
        TwentyMinutes = 20,
        ThirtyMinutes = 30,
        OneHour = 60,
        OneDay = 1440
    }

    public class Config
    {
        //public static CacheLevel DefaultCacheLevel { get { return CacheConfig.DefaultCacheLevel; } set { CacheConfig.DefaultCacheLevel = value; } }
        //public static int DefaultCacheTimeout { get { return CacheConfig.DefaultCacheTimeout; } set { CacheConfig.DefaultCacheTimeout = value; } }

        public static Settings GlobalSettings { get; private set; }


        //private static CacheLevel cacheLevel;
        //public static CacheLevel CacheLevel 
        //{ 
        //    get
        //    {
        //        return cacheLevel;
        //    }
        //    set 
        //    {
        //        cacheLevel = value;

        //        if (cacheLevel == Modl.CacheLevel.Off)
        //        {
        //            AsyncDbAccess.DisposeAllWorkers();
        //            CacheManager.Clear();
        //        }
        //    }
        //}

        static Config()
        {
            GlobalSettings = new Settings();
        }
    }
}
