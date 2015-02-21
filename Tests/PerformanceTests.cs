using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using System.Diagnostics;
using System.Threading.Tasks;
using Modl.Db;

namespace Tests
{
    [TestClass]
    public class PerformanceTests
    {
        List<Database> databases;
        Basics basics = new Basics();
        
        public PerformanceTests()
        {
            //Config.CacheLevel = CacheLevel.Off;

            databases = new List<Database>
            {
                //Database.Get("SqlServerCeDb"),
                //Database.Get("MySQLDb"),
                Database.Get("SqlServerDb")
            };
        }
        
        ////[TestMethod]
        //public void RunAllTest()
        //{
        //    var watch = Stopwatch.StartNew();

        //    CRUDPerformanceTest(1000);
        //    //AsyncCRUDPerformanceTest(1000, 10);
            
        //    Console.WriteLine(string.Format("All tests: {0} ms.", watch.Elapsed.TotalMilliseconds));
        //    Database.DisposeAll();
        //    watch.Stop();
        //    Console.WriteLine(string.Format("With dispose: {0} ms.", watch.Elapsed.TotalMilliseconds));
        //}
        

        ////[TestMethod]
        //public void CRUDPerformanceTest(int iterations = 100)
        //{
        //    foreach (var db in databases)
        //    {
        //        TimeMethod(db, iterations, CacheLevel.On, basics.CRUD);
        //        TimeMethod(db, iterations, CacheLevel.On, basics.CRUDExplicitId);
        //    }
        //}

        ////[TestMethod]
        //public void AsyncCRUDPerformanceTest(int iterations = 100, int threads = 10)
        //{
        //    foreach (var db in databases)
        //    {
        //        TimeMethodAsync(db, iterations, CacheLevel.On, basics.CRUD, threads);
        //        TimeMethodAsync(db, iterations, CacheLevel.On, basics.CRUDExplicitId, threads);
        //    }
        //}

        //public TimeSpan TimeMethod(Database db, int iterations, CacheLevel cache, Action<Database> testMethod)
        //{
        //    Config.DefaultCacheLevel = cache;
        //    var watch = Stopwatch.StartNew();

        //    for (int i = 0; i < iterations; i++)
        //        testMethod.Invoke(db);

        //    watch.Stop();
        //    Console.WriteLine(string.Format("{4}, {0} iterations, {1}: {2} ms. (cache {3})", iterations, db.Name, watch.Elapsed.TotalMilliseconds, cache, testMethod.Method.Name));

        //    return watch.Elapsed;
        //}

        //public void TimeMethodAsync(Database db, int iterations, CacheLevel cache, Action<Database> testMethod, int threads)
        //{
        //    Config.DefaultCacheLevel = cache;
        //    var watch = Stopwatch.StartNew();

        //    Parallel.For(0, threads, i =>
        //    {
        //        TimeMethod(db, iterations, cache, testMethod);
        //    });

        //    watch.Stop();
        //    Console.WriteLine(string.Format("Async total, {0} threads, {1}: {2} ms. Avg: {3} ms.", threads, db.Name, watch.Elapsed.TotalMilliseconds, watch.Elapsed.TotalMilliseconds / threads));
        //    Console.WriteLine();
        //}
    }
}
