using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Query;
using System.Data.Common;

namespace Modl.DataAccess
{
    public static class AsyncDbAccess
    {
        private static Dictionary<Database, AsyncWorker> workers = new Dictionary<Database, AsyncWorker>();

        private static AsyncWorker GetWorker(Database database)
        {
            if (workers.ContainsKey(database))
                return workers[database];

            workers[database] = new AsyncWorker(database);
            return workers[database];
        }

        public static void CloseAllWorkers()
        {
            foreach (var worker in workers.Values)
                worker.Dispose();

            workers.Clear();
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

            return DbAccess.ExecuteReader(query.ToDbCommand());
        }
    }
}
