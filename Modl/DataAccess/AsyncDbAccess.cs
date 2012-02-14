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
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Modl.Query;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Modl.DataAccess
{
    public static class AsyncDbAccess
    {
        private static readonly object workLock = new object();
        private static Dictionary<Database, AsyncWorker> workers = new Dictionary<Database, AsyncWorker>();

        static AsyncDbAccess()
        {
            StartWorkManager();
        }

        private static void StartWorkManager()
        {
            Thread thread = new Thread(WorkManagerLoop);

            thread.IsBackground = true;
            thread.Start(thread);
        }

        private static void WorkManagerLoop(object thread)
        {
            while (true)
            {
                lock (workLock)
                {
                    foreach (var worker in workers.Values)
                    {
                        int workersToStart = (int)Math.Ceiling((worker.QueueDepth - (worker.RunningWorkers * 5000)) / 5000.0);

                        if (workersToStart > 0)
                            worker.StartWorker(workersToStart);
                    }
                }
                
                Thread.Sleep(1000);
            }
        }

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

        public static Task<bool> ExecuteNonQuery(Database database, params IQuery[] queries)
        {
            //if (Config.DefaultCacheLevel == CacheLevel.Off)
            //    DbAccess.ExecuteNonQuery(queries);
            //else
            //{
                //foreach (var list in queries.GroupBy(x => x.DatabaseProvider))
                //{
                    //new WorkPackage<object>(WorkType.Write, list.ToArray()).DoWorkAsync();
            var work = new WorkPackage<bool>(WorkType.Write, queries);
            GetWorker(database).Enqueue(work);
            
            return work.AwaitResult();
                //}
            //}
        }

        public static Task<T> ExecuteScalar<T>(Database database, bool onQueue, params IQuery[] queries)
        {
            var work = new WorkPackage<T>(WorkType.Scalar, queries);

            if (/*Config.DefaultCacheLevel == CacheLevel.Off ||*/ !onQueue)
                return work.DoWorkAsync();
            else
            {
                GetWorker(database).Enqueue(work);
                return work.AwaitResult();
            }

            //return Task<T>.Factory.StartNew(() => DbAccess.ExecuteScalar<T>(queries));
        }

        static public Task<DbDataReader> ExecuteReader(IQuery query, bool onQueue = true)
        {
            var work = new WorkPackage<DbDataReader>(WorkType.Read, query);

            if (/*Config.DefaultCacheLevel == CacheLevel.Off ||*/ !onQueue)
                return work.DoWorkAsync();
            else
            {
                GetWorker(query.DatabaseProvider).Enqueue(work);
                return work.AwaitResult();
            }
        }
    }
}
