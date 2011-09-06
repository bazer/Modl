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
using Modl.Fields;

namespace Modl.Cache
{
    internal static class StaticCache<M, IdType>
        where M : Modl<M, IdType>, new()
    {
        private const bool writeDebugText = false;

        private static ConcurrentDictionary<Database, ConcurrentDictionary<IdType, M>> cache = new ConcurrentDictionary<Database, ConcurrentDictionary<IdType, M>>();
        private static ConcurrentDictionary<Database, ConcurrentHashSet<M>> preliminaryCache = new ConcurrentDictionary<Database, ConcurrentHashSet<M>>();
        private static ConcurrentDictionary<Database, ConcurrentHashSet<IdType>> deleted = new ConcurrentDictionary<Database, ConcurrentHashSet<IdType>>();

        static StaticCache()
        {
            Initialize();
            CacheConfig.RegisterClearMethod(Clear);
        }

        internal static void Initialize()
        {
            foreach (var database in Database.GetAll())
            {
                cache.TryAdd(database, new ConcurrentDictionary<IdType, M>());
                deleted.TryAdd(database, new ConcurrentHashSet<IdType>());
                preliminaryCache.TryAdd(database, new ConcurrentHashSet<M>());
            }
        }

        internal static void Clear()
        {
            cache.Clear();
            deleted.Clear();
            preliminaryCache.Clear();

            Initialize();
        }

        internal static bool Contains(IdType id, Database database)
        {
            return cache[database].ContainsKey(id);
        }

        internal static bool DeletedContains(IdType id, Database database)
        {
            return deleted[database].Contains(id);
        }

        internal static M Get(IdType id, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache Get: {1}", database.Name, id);

            if (DeletedContains(id, database))
                return null;
            else
                return cache[database].GetOrAdd(id, x => new Select<M, IdType>(database).Where(Modl<M, IdType>.IdName).EqualTo(x).Get());
        }

        internal static Task<M> GetAsync(IdType id, Database database)
        {
            return Task<M>.Factory.StartNew(() =>
                {
                    return Get(id, database);
                });
        }

        internal static M GetWhere(Expression<Func<M, bool>> query, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache GetWhere: {1}", database.Name, query.ToString());

            var m = cache[database].Values.Where(query.Compile()).FirstOrDefault(x => !DeletedContains(x.Id, database));

            if (m != null)
                return m;

            m = new Select<M, IdType>(database, query).GetAll().FirstOrDefault(x => !DeletedContains(x.Id, database));

            if (m != null)
                cache[database].GetOrAdd(m.Id, m);

            return m;
        }

        internal static IEnumerable<M> GetAll(Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache GetAll", database.Name);

            var cacheList = AllInCache(database);
            var resultList = new HashSet<IdType>();

            foreach (var m in new Select<M, IdType>(database).WhereNotAny(cacheList.Where(x => x.IsIdLoaded)).WhereNotAny(deleted[database]).GetAll())
            {
                if (writeDebugText)
                    Console.WriteLine("[{0}] Cache calling Add: {1}", database.Name, m.Id);
                Add(m.Id, m, database);
                if (writeDebugText)
                    Console.WriteLine("[{0}] DB Return: {1}", database.Name, m.Id);
                yield return m;

                resultList.Add(m.Id);
            }

            foreach (var m in cacheList)
            {
                if (/*!m.IsIdLoaded ||*/ !resultList.Contains(m.Id))
                {
                    if (writeDebugText)
                        Console.WriteLine("[{0}] Cache Return", database.Name);

                    yield return m;
                }
            }
        }

        internal static IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache GetAllWhere: {1}", database.Name, query.ToString());

            var q = query.Compile();
            var cacheList = AllInCache(database);
            var resultList = new HashSet<IdType>();

            foreach (var m in new Select<M, IdType>(database, query).WhereNotAny(cacheList.Where(x => x.IsIdLoaded)).WhereNotAny(deleted[database]).GetAll())
            {
                Add(m.Id, m, database);
                yield return m;

                resultList.Add(m.Id);
            }

            foreach (var m in cacheList.Where(q))
            {
                if (/*!m.IsIdLoaded ||*/ !resultList.Contains(m.Id))
                {
                    if (writeDebugText)
                        Console.WriteLine("[{0}] Cache Return: {1}", database.Name, m.Id);

                    yield return m;
                }
            }
        }

        private static HashSet<M> AllInCache(Database database)
        {
            var list = new HashSet<M>(preliminaryCache[database]);
            foreach (var m in cache[database].Values)
                list.Add(m);

            return list;
        }

        internal static void Add(IdType id, M instance, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache Add: {1}", database.Name, id);

            if (Statics<M, IdType>.CacheLevel == CacheLevel.On)
            {
                deleted[database].TryRemove(id);
                cache[database].GetOrAdd(id, instance);
                preliminaryCache[database].TryRemove(instance);

                //if (deleted[database].Contains(id))
                //    deleted[database].Remove(id);

                //if (preliminaryCache[database].Contains(instance))
                //    preliminaryCache[database].Remove(instance);
            }
        }

        internal static void AddPreliminary(M instance, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache AddPreliminary", database.Name);

            if (Statics<M, IdType>.CacheLevel == CacheLevel.On)
            {
                preliminaryCache[database].Add(instance);
            }
        }

        internal static void Delete(IdType id, Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache Delete: {1}", database.Name, id);

            if (Statics<M, IdType>.CacheLevel == CacheLevel.On)
            {
                deleted[database].Add(id);
                M m;
                cache[database].TryRemove(id, out m);
            }
        }

        internal static void DeleteAll(Database database)
        {
            if (writeDebugText)
                Console.WriteLine("[{0}] Cache DeleteAll", database.Name);

            if (Statics<M, IdType>.CacheLevel == CacheLevel.On)
            {
                deleted[database] = new ConcurrentHashSet<IdType>(new Select<M, IdType>(database).GetMaterializer().GetIds());
                deleted[database].AddRange(AllInCache(database).Select(x => x.Id));

                //foreach (var m in cache[database].Values)
                //    deleted[database].Add(m.Id);

                //foreach (var m in preliminaryCache[database])
                //    deleted[database].Add(m.Id);

                cache[database].Clear();
                preliminaryCache[database].Clear();
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
