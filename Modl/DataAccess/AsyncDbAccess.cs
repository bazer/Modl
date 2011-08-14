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
using System.Data.Common;
using System.Linq;
using Modl.Query;

namespace Modl.DataAccess
{
    public static class AsyncDbAccess
    {
        private static readonly object workLock = new object();
        private static Dictionary<Database, AsyncWorker> workers = new Dictionary<Database, AsyncWorker>();

        private static AsyncWorker GetWorker(Database database)
        {
            if (workers.ContainsKey(database))
                return workers[database];

            lock (workLock)
            {
                workers[database] = new AsyncWorker(database);
            }

            return workers[database];
        }

        public static void DisposeWorker(Database database)
        {
            lock (workLock)
            {
                if (workers.ContainsKey(database))
                {
                    workers[database].Dispose();
                    workers.Remove(database);
                }
            }
        }

        public static void DisposeAllWorkers()
        {
            lock (workLock)
            {
                foreach (var worker in workers.Values)
                    worker.Dispose();

                workers.Clear();
            }
        }

        public static void ExecuteNonQuery(params IQuery[] queries)
        {
            if (Config.CacheLevel == CacheLevel.Off)
                DbAccess.ExecuteNonQuery(queries);
            else
            {
                foreach (var list in queries.GroupBy(x => x.DatabaseProvider))
                {
                    GetWorker(list.Key).Enqueue(list.ToArray());
                }
            }
        }

        static public DbDataReader ExecuteReader(IQuery query)
        {
            if (Config.CacheLevel == CacheLevel.On)
                GetWorker(query.DatabaseProvider).WaitToCompletion();

            return DbAccess.ExecuteReader(query).First();
        }
    }
}
