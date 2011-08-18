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
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace Modl.Cache
{
    internal static class StaticCache<M, IdType>
        where M : Modl<M, IdType>, new()
    {
        private const bool writeDebugText = false;

        private static readonly object deleteLock = new object();
        private static readonly object preliminaryLock = new object();
        //private static Dictionary<Database, Dictionary<IdType, M>> cache = new Dictionary<Database, Dictionary<IdType, M>>();
        private static Dictionary<Database, AsyncCache<IdType, M>> cache = new Dictionary<Database, AsyncCache<IdType, M>>();
        private static Dictionary<Database, HashSet<IdType>> deleted = new Dictionary<Database, HashSet<IdType>>();
        private static ConcurrentDictionary<Database, HashSet<M>> preliminaryCache = new ConcurrentDictionary<Database, HashSet<M>>();

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
                cache.Add(database, new AsyncCache<IdType, M>(id => new Select<M, IdType>(database).Where(Modl<M, IdType>.IdName).EqualTo(id).GetAsync(false)));
                deleted.Add(database, new HashSet<IdType>());

                lock (preliminaryLock)
                {
                    preliminaryCache.TryAdd(database, new HashSet<M>());
                }
            }
        }

        internal static void Clear()
        {
            cache.Clear();

            lock (deleteLock)
                deleted.Clear();

            lock (preliminaryLock)
                preliminaryCache.Clear();

            Initialize();
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

        internal static Task<M> Get(IdType id, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache Get: {1}", database.Name, id);

            if (DeletedContains(id, database))
                return null;
            else
                return cache[database].GetValue(id);
        }

        internal static M GetWhere(Expression<Func<M, bool>> query, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache GetWhere: {1}", database.Name, query.ToString());

            var m = cache[database].GetValuesWhere(query.Compile()).FirstOrDefault(x => !DeletedContains(x.Id, database));

            if (m != null)
                return m;

            m = new Select<M, IdType>(database, query).GetAll().FirstOrDefault(x => !DeletedContains(x.Id, database));

            if (m != null)
                cache[database].SetValue(m.Id, m);

            return m;
        }

        internal static IEnumerable<M> GetAll(Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache GetAll", database.Name);

            HashSet<IdType> list = new HashSet<IdType>();

            foreach (var m in cache[database].GetAllValues())
            {
                if (!DeletedContains(m.Id, database))
                {
                    //Add(m.Id, m, database);

                    list.Add(m.Id);
                    if (writeDebugText)
                        Console.WriteLine("[{0}] Cache Return: {1}", database.Name, m.Id);
                    yield return m;
                }
            }

            List<M> prel;
            lock (preliminaryLock)
            {
                prel = preliminaryCache[database].ToList();
            }

            foreach (var m in prel)
            {
                list.Add(m.Id);
                if (writeDebugText)
                    Console.WriteLine("[{0}] Prel Return: {1}", database.Name, m.Id);
                yield return m;
            }


            foreach (var m in new Select<M, IdType>(database).GetAll())
            {
                if (!list.Contains(m.Id) && !DeletedContains(m.Id, database))
                {
                    if (writeDebugText)
                        Console.WriteLine("[{0}] Cache calling Add: {1}", database.Name, m.Id);
                    Add(m.Id, m, database);
                    if (writeDebugText)
                        Console.WriteLine("[{0}] DB Return: {1}", database.Name, m.Id);
                    yield return m;
                }
            }
        }

        internal static IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache GetAllWhere: {1}", database.Name, query.ToString());

            var q = query.Compile();
            HashSet<IdType> list = new HashSet<IdType>();

            foreach (var m in cache[database].GetValuesWhere(q))
            {
                if (!DeletedContains(m.Id, database))
                {
                    //Add(m.Id, m, database);
                    list.Add(m.Id);
                    yield return m;
                }
            }

            List<M> prel;
            lock (preliminaryLock)
            {
                prel = preliminaryCache[database].Where(q).ToList();
            }

            foreach (var m in prel)
            {
                list.Add(m.Id);
                yield return m;
            }

            foreach (var m in new Select<M, IdType>(database, query).GetAll())
            {
                if (!list.Contains(m.Id) && !DeletedContains(m.Id, database))
                {
                    Add(m.Id, m, database);
                    yield return m;
                }
            }
        }

        internal static void Add(IdType id, M instance, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache Add: {1}", database.Name, id);

            if (Config.CacheLevel == CacheLevel.On)
            {
                cache[database].SetValue(id, instance);

                if (DeletedContains(id, database))
                {
                    lock (deleteLock)
                        deleted[database].Remove(id);
                }

                lock (preliminaryLock)
                {
                    if (preliminaryCache[database].Contains(instance))
                        preliminaryCache[database].Remove(instance);
                }
            }
        }

        internal static void AddPreliminary(M instance, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache AddPreliminary", database.Name);

            if (Config.CacheLevel == CacheLevel.On)
            {
                lock (preliminaryLock)
                {
                    preliminaryCache[database].Add(instance);
                }
            }
        }

        internal static void Delete(IdType id, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache Delete: {1}", database.Name, id);

            if (Config.CacheLevel == CacheLevel.On)
            {
                //if (CacheContains(id, database))
                //    cache[database].Remove(id);

                cache[database].RemoveValue(id);

                lock (deleteLock)
                {
                    //if (!deleted.ContainsKey(database))
                    //    deleted.Add(database, new HashSet<IdType>());

                    deleted[database].Add(id);
                }

            }
        }

        internal static void DeleteAll(Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache DeleteAll", database.Name);

            if (Config.CacheLevel == CacheLevel.On)
            {


                lock (deleteLock)
                {
                    //if (!deleted.ContainsKey(database))
                    //    deleted.Add(database, new HashSet<IdType>());

                    deleted[database] = new HashSet<IdType>(new Select<M, IdType>(database).GetMaterializer(false).GetIds()); //(IdType)Convert.ChangeType(x.Id, typeof(IdType))));
                }

                cache[database].Clear();

                lock (preliminaryLock)
                {
                    preliminaryCache[database].Clear();
                }
            }
        }



        //private static M ReturnNullOrThrow(bool throwExceptionOnNotFound)
        //{
        //    if (throwExceptionOnNotFound)
        //        throw new RecordNotFoundException();
        //    else
        //        return null;
        //}
    }
}
