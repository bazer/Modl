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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Modl.Query;

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
