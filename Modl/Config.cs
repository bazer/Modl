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
using Modl.DatabaseProviders;
using System.Data;
using Modl.DataAccess;
using Modl.Cache;

namespace Modl
{
    public enum CacheLevel
    {
        On,
        Off,
        All
    }

    public enum Timeout
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
        public static CacheLevel DefaultCacheLevel { get { return CacheConfig.DefaultCacheLevel; } set { CacheConfig.DefaultCacheLevel = value; } }
        public static int DefaultCacheTimeout { get { return CacheConfig.DefaultCacheTimeout; } set { CacheConfig.DefaultCacheTimeout = value; } }


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

        
        protected static Dictionary<string, Database> DatabaseProviders = new Dictionary<string, Database>();

        static Config()
        {
            DefaultCacheLevel = CacheLevel.On;
            DefaultCacheTimeout = 20;

            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
                if (!string.IsNullOrWhiteSpace(connString.ConnectionString) && !string.IsNullOrWhiteSpace(connString.Name) && !string.IsNullOrWhiteSpace(connString.ProviderName))
                    Database.AddFromConnectionString(connString);

            //if (ConfigurationManager.ConnectionStrings.Count > 1)
            //    AddDatabaseProvider(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1]);
        }

        

        private static Database defaultDbProvider = null;
        internal static Database DefaultDatabase
        {
            get
            {
                if (defaultDbProvider == null)
                    defaultDbProvider = Config.DatabaseProviders.Last().Value;

                return defaultDbProvider;
            }
            set
            {
                defaultDbProvider = value;
            }
        }

        internal static Database AddDatabase(Database database)
        {
            DatabaseProviders[database.Name] = database;

            return database;
        }

        internal static Database GetDatabase(string databaseName)
        {
            return DatabaseProviders[databaseName];
        }

        internal static List<Database> GetAllDatabases()
        {
            return DatabaseProviders.Values.ToList();
        }

        internal static void RemoveDatabase(string databaseName)
        {
            DatabaseProviders.Remove(databaseName);
        }

        internal static void RemoveAllDatabases()
        {
            DatabaseProviders.Clear();
        }

        //public static IDbConnection GetConnection(string databaseName)
        //{
        //    return DatabaseProviders[databaseName].GetConnection();
        //}

        
    }
}
