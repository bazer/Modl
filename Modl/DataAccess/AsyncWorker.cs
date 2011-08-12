using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Modl.Query;
using Modl.DataAccess;

namespace Modl.DataAccess
{
    internal class AsyncWorker : IDisposable
    {
        private Database database;
        private bool DoWork = true;
        private Thread thread;
        private List<IQuery> workerQueue = new List<IQuery>();

        internal AsyncWorker(Database database)
        {
            this.database = database;

            thread = new Thread(WorkerLoop);
            thread.IsBackground = false;
            thread.Start();
        }

        internal void Enqueue(params IQuery[] queries)
        {
            lock (workerQueue)
            {
                workerQueue.AddRange(queries);
            }
        }

        internal void WaitToCompletion()
        {
            while (workerQueue.Count != 0)
                Thread.Sleep(20);
        }

        private void WorkerLoop()
        {
            while (DoWork || workerQueue.Count != 0)
            {
                IQuery[] list;

                try
                {
                    if (workerQueue.Count != 0)
                    {
                        lock (workerQueue)
                        {
                            int i = 0;
                            list = workerQueue.TakeWhile(x => (i += x.ParameterCount) < 2000).ToArray();
                            workerQueue.RemoveRange(0, list.Length);
                        }

                        DbAccess.ExecuteNonQuery(list);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
                }

                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            DoWork = false;
            thread.Join();
        }
    }
}
