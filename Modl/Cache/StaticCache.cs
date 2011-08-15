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
using System.Collections.Generic;
using System.Linq;
using Modl.Exceptions;
using Modl.Query;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Modl.Cache
{
    internal static class StaticCache<M, IdType>
        where M : Modl<M>, new()
    {
        private static readonly object workLock = new object();
        //private static Dictionary<Database, Dictionary<IdType, M>> cache = new Dictionary<Database, Dictionary<IdType, M>>();
        private static Dictionary<Database, AsyncCache<IdType, M>> cache = new Dictionary<Database, AsyncCache<IdType, M>>();
        private static Dictionary<Database, HashSet<IdType>> deleted = new Dictionary<Database, HashSet<IdType>>();

        //AsyncCache<IdType, M> cache;

        static StaticCache()
        {
            Initialize();
            CacheManager.RegisterClearMethod(Clear);
        }

        internal static void Initialize()
        {
            foreach (var database in Database.GetAll())
            {
                cache.Add(database, new AsyncCache<IdType, M>(x => new Task<M>(() => new Select<M>(database).Where(Modl<M>.IdName).EqualTo(x).Get(false))));
                deleted.Add(database, new HashSet<IdType>());
            }
        }

        internal static void Clear()
        {
            lock (workLock)
            {
                cache.Clear();
                deleted.Clear();

                Initialize();
            }
        }

        //internal static bool CacheContains(IdType id, Database database)
        //{
        //    return cache.ContainsKey(database) && cache[database].ContainsKey(id);
        //}

        internal static bool DeletedContains(IdType id, Database database)
        {
            //lock (workLock)
            //{
                return deleted[database].Contains(id);
            //}
        }

        internal static M Get(IdType id, Database database, bool throwExceptionOnNotFound)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                //if (CacheContains(id, database))
                //    return cache[database][id];
                if (DeletedContains(id, database))
                    return ReturnNullOrThrow(throwExceptionOnNotFound);
                else
                    return cache[database].GetValue(id).Result;
            }

            M instance = new Select<M>(database).Where(Modl<M>.IdName).EqualTo(id).Get(throwExceptionOnNotFound);

            //if (Config.CacheLevel == CacheLevel.On)
            //    Add(id, instance, database, throwExceptionOnNotFound);

            return instance;
        }

        internal static void Add(IdType id, M instance, Database database)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                //lock (workLock)
                //{
                //    if (!cache.ContainsKey(database))
                //        cache.Add(database, new AsyncCache<IdType,M>(x => new Task<M>(() => new Select<M>(database).Where(Modl<M>.IdName).EqualTo(x).Get(throwExceptionOnNotFound))));
                //        //cache.Add(database, new Dictionary<IdType, M>());
                //}

                //cache[database][id] = instance;
                cache[database].SetValue(id, instance);

                if (DeletedContains(id, database))
                    deleted[database].Remove(id);
            }
        }

        internal static void Delete(IdType id, Database database)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                //if (CacheContains(id, database))
                //    cache[database].Remove(id);

                lock (workLock)
                {
                    //if (!deleted.ContainsKey(database))
                    //    deleted.Add(database, new HashSet<IdType>());

                    deleted[database].Add(id);
                }
                
            }
        }

        internal static void DeleteAll(Database database)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                cache[database].Clear();

                lock (workLock)
                {
                    //if (!deleted.ContainsKey(database))
                    //    deleted.Add(database, new HashSet<IdType>());

                    deleted[database] = new HashSet<IdType>(Modl<M>.GetAll(database).Select(x => (IdType)Convert.ChangeType(x.Id, typeof(IdType))));
                }
            }
        }

        

        private static M ReturnNullOrThrow(bool throwExceptionOnNotFound)
        {
            if (throwExceptionOnNotFound)
                throw new RecordNotFoundException();
            else
                return null;
        }
    }
}
