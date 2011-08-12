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

namespace Modl.Cache
{
    internal static class StaticCache<M, IdType>
        where M : Modl<M>, new()
    {
        private static Dictionary<Database, Dictionary<IdType, M>> cache = new Dictionary<Database, Dictionary<IdType, M>>();
        private static Dictionary<Database, HashSet<IdType>> deleted = new Dictionary<Database, HashSet<IdType>>();

        static StaticCache()
        {
            CacheManager.RegisterClearMethod(Clear);
        }

        internal static bool CacheContains(IdType id, Database database)
        {
            return cache.ContainsKey(database) && cache[database].ContainsKey(id);
        }

        internal static bool DeletedContains(IdType id, Database database)
        {
            return deleted.ContainsKey(database) && deleted[database].Contains(id);
        }

        internal static M Get(IdType id, Database database, bool throwExceptionOnNotFound)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                if (CacheContains(id, database))
                    return cache[database][id];
                else if (DeletedContains(id, database))
                    return ReturnNullOrThrow(throwExceptionOnNotFound);
            }

            M instance = new Select<M>(database).Where(Modl<M>.IdName).EqualTo(id).Get(throwExceptionOnNotFound);

            if (Config.CacheLevel == CacheLevel.On)
                Add(id, instance, database);

            return instance;
        }

        internal static void Add(IdType id, M instance, Database database)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                if (!cache.ContainsKey(database))
                    cache.Add(database, new Dictionary<IdType, M>());

                cache[database][id] = instance;
            }
        }

        internal static void Delete(IdType id, Database database)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                if (CacheContains(id, database))
                    cache[database].Remove(id);

                if (!deleted.ContainsKey(database))
                    deleted.Add(database, new HashSet<IdType>());

                deleted[database].Add(id);
            }
        }

        internal static void DeleteAll(Database database)
        {
            if (Config.CacheLevel == CacheLevel.On)
            {
                cache.Clear();

                if (!deleted.ContainsKey(database))
                    deleted.Add(database, new HashSet<IdType>());

                deleted[database] = new HashSet<IdType>(Modl<M, IdType>.GetAll(database).Select(x => x.Id).Cast<IdType>());
            }
        }

        internal static void Clear()
        {
            cache.Clear();
            deleted.Clear();
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
