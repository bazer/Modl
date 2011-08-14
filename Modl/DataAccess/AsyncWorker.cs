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
using System.Collections.Concurrent;

namespace Modl.DataAccess
{
    //internal class AsyncWorker : IDisposable
    //{
    //    private Database database;
    //    private BlockingCollection<IQuery> workerQueue = new BlockingCollection<IQuery>();
    //    private List<Thread> workers = new List<Thread>();


    //    //private Thread thread;
    //    private volatile bool doWork = true;
    //    private volatile bool isWorking = true;
    //    private readonly object workLock = new object();
    //    private bool isDisposed = false;

    //    internal AsyncWorker(Database database)
    //    {
    //        this.database = database;

    //        for (int i = 0; i < 1; i++)
    //        {
    //            Thread thread = new Thread(WorkerLoop);
    //            workers.Add(thread);

    //            thread.IsBackground = false;
    //            thread.Start(i);
    //        }
    //    }

    //    internal void Enqueue(params IQuery[] queries)
    //    {
    //        isWorking = true;
    //        foreach (var query in queries)
    //            workerQueue.Add(query);

    //        //lock (workLock)
    //        //{
    //        //    isWorking = true;
    //        //    workerQueue.AddRange(queries);
    //        //    Monitor.Pulse(workLock);
    //        //}
    //    }

    //    internal void WaitToCompletion()
    //    {
    //        while (isWorking)
    //            Thread.Yield();
    //    }

    //    private void WorkerLoop(object threadNumber)
    //    {
    //        int num = (int)threadNumber;

    //        while (!workerQueue.IsCompleted)
    //        {
    //            try
    //            {
    //                //IQuery[] list;

    //                //lock (workLock)
    //                //{
    //                if (workerQueue.Count == 0)
    //                {
    //                    isWorking = false;
    //                    workers[num].IsBackground = true;
    //                    //Monitor.Wait(workLock);
    //                    //thread.IsBackground = false;
    //                }

                    
    //                List<IQuery> list = new List<IQuery>();
    //                int i = 0;
    //                IQuery query;

    //                do
    //                {
    //                    if (list.Count == 0)
    //                    {
    //                        query = workerQueue.Take();
    //                        workers[num].IsBackground = false;
    //                    }
    //                    else
    //                        workerQueue.TryTake(out query, 50);

    //                    if (query != null)
    //                    {
    //                        i += query.ParameterCount;
    //                        list.Add(query);
    //                    }
    //                }
    //                while (query != null && i < 2000 && !workerQueue.IsCompleted);
                  
    //                //list = workerQueue.TakeWhile(x => (i += x.ParameterCount) < 2000).ToArray();
                    
    //                    //workerQueue.RemoveRange(0, list.Length);
    //                //}

    //                if (list.Count != 0)
    //                    DbAccess.ExecuteNonQuery(list);
    //            }
    //            catch (Exception e)
    //            {
    //                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
    //            }
    //        }
    //    }

        
    //    public void Dispose()
    //    {
    //        try
    //        {
    //            if (!isDisposed)
    //            {
    //                Console.WriteLine("Dispose: {0}, Worker queue: {1}", DateTime.Now.ToLongTimeString(), workerQueue.Count);

    //                isDisposed = true;

    //                workerQueue.CompleteAdding();

    //                //lock (workLock)
    //                //{
    //                //    doWork = false;
    //                //    Monitor.Pulse(workLock);
    //                //}
                   
    //                foreach (var worker in workers)
    //                    worker.Join();
    //            }

    //            Console.WriteLine("End dispose: {0}, Worker queue: {1}", DateTime.Now.ToLongTimeString(), workerQueue.Count);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
    //        }
    //    }

    //    ~AsyncWorker()
    //    {
    //        Dispose();
    //    }

    internal class AsyncWorker : IDisposable
    {
        private const bool writeDebugText = false;

        private Database database;
        private ConcurrentQueue<IQuery> workerQueue = new ConcurrentQueue<IQuery>();
        //private List<IQuery> workerQueue = new List<IQuery>();
        private List<Thread> workers = new List<Thread>();
        private volatile int runningWorkers = 0;
        private int maxWorkers = 100;

        //private Thread thread;
        private volatile bool doWork = true;
        private volatile bool isWorking = false;
        private readonly object workLock = new object();
        private bool isDisposed = false;

        internal AsyncWorker(Database database)
        {
            this.database = database;

            //for (int i = 0; i < 10; i++)
            //{
            //    Thread thread = new Thread(WorkerLoop);
            //    workers.Add(thread);

            //    thread.IsBackground = false;
            //    //thread.Start(i);
            //}

            //thread = new Thread(WorkerLoop);
            //thread.IsBackground = false;
            //thread.Start();
        }

        private void StartWorker()
        {
            lock (workLock)
            {
                if (runningWorkers < maxWorkers)
                {
                    Thread thread = new Thread(WorkerLoop);
                    workers.Add(thread);

                    runningWorkers++;

                    thread.IsBackground = false;
                    thread.Start(thread);

                    if (writeDebugText)
                        Console.WriteLine("[{0}]New worker: {1}, Queue: {2}", database.Name, runningWorkers, workerQueue.Count);
                }   
            }
        }

        internal void Enqueue(params IQuery[] queries)
        {
            foreach (var query in queries)
                workerQueue.Enqueue(query);

            isWorking = true;
            
            if (runningWorkers == 0)
                StartWorker();
            
            //lock (workLock)
            //{
            //    //workerQueue.AddRange(queries);
            //    Monitor.Pulse(workLock);
            //}
        }

        internal void WaitToCompletion()
        {
            while (isWorking)
                Thread.Yield();
        }

        private void WorkerLoop(object thread)
        {
            int sleepCycles = 0;
            //int num = (int)threadNumber;

            while ((doWork && sleepCycles < 1000) || workerQueue.Count != 0)
            {
                try
                {
                    //IQuery[] list;
                    List<IQuery> list = new List<IQuery>();


                    if (workerQueue.Count == 0)
                    {
                        isWorking = false;
                        //workers[num].IsBackground = true;
                        sleepCycles += 100;
                        Thread.Sleep(100);
                        //lock (workLock)
                        //{
                        //    Monitor.Wait(workLock);
                        //}
                        //workers[num].IsBackground = false;
                    }
                    else if (workerQueue.Count / runningWorkers > 1000)
                        StartWorker();


                    int i = 0;
                    while (i < 100)
                    {
                        IQuery query;
                        if (workerQueue.TryDequeue(out query))
                        {
                            list.Add(query);
                            i += query.ParameterCount;
                        }
                        else
                            break;
                    }
                    
                    //int i = 0;
                    //list = workerQueue.TakeWhile(x => (i += x.ParameterCount) < 200).ToArray();
                    //workerQueue.RemoveRange(0, list.Length);


                    if (list.Count != 0)
                    {
                        //DbAccess.ExecuteNonQuery(list);
                        foreach (var reader in DbAccess.ExecuteReader(list))
                            reader.Close();

                        sleepCycles = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
                }
            }

            workers.Remove((Thread)thread);
            runningWorkers--;
            
            if (writeDebugText)
                Console.WriteLine("[{0}]Worker exit: {1}, Queue: {2}", database.Name, runningWorkers, workerQueue.Count);
        }


        public void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    if (writeDebugText)
                        Console.WriteLine("[{0}]Dispose: {1}, Queue: {2}", database.Name, DateTime.Now.ToLongTimeString(), workerQueue.Count);

                    isDisposed = true;

                    //lock (workLock)
                    //{
                        doWork = false;
                    //    Monitor.PulseAll(workLock);
                    //}

                    //thread.Join();
                    //foreach (var worker in workers)
                    //    worker.Join();
                    while (runningWorkers != 0)
                        Thread.Yield();

                    //workers.ForEach(x => x.Join());

                    //for (int i = workers.Count; i > 0; i--)
                    //    workers[i - 1].Join();
                }

                if (writeDebugText)
                    Console.WriteLine("[{0}]End dispose: {1}, Queue: {2}", database.Name, DateTime.Now.ToLongTimeString(), workerQueue.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
            }
        }

        ~AsyncWorker()
        {
            Dispose();
        }
    }
}
