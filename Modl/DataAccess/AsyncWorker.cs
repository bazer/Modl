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
        private List<IQuery> workerQueue = new List<IQuery>();

        private Thread thread;
        private volatile bool doWork = true;
        private volatile bool isWorking = true;
        private readonly object workLock = new object();


        internal AsyncWorker(Database database)
        {
            this.database = database;

            thread = new Thread(WorkerLoop);
            thread.IsBackground = false;
            thread.Start();
        }

        internal void Enqueue(params IQuery[] queries)
        {
            lock (workLock)
            {
                isWorking = true;
                workerQueue.AddRange(queries);
                Monitor.Pulse(workLock);
            }
        }

        internal void WaitToCompletion()
        {
            while (isWorking)
                Thread.Yield();
        }

        private void WorkerLoop()
        {
            while (doWork || workerQueue.Count != 0)
            {
                try
                {
                    IQuery[] list;

                    lock (workLock)
                    {
                        if (workerQueue.Count == 0)
                        {
                            isWorking = false;
                            Monitor.Wait(workLock);
                        }

                        int i = 0;
                        list = workerQueue.TakeWhile(x => (i += x.ParameterCount) < 2000).ToArray();
                        workerQueue.RemoveRange(0, list.Length);
                    }

                    if (list.Length != 0)
                        DbAccess.ExecuteNonQuery(list);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
                }
            }
        }

        public void Dispose()
        {
            lock (workLock)
            {
                doWork = false;
                Monitor.Pulse(workLock);
            }
            
            thread.Join();
        }
    }
}
