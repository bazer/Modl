//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;
//using ExampleModel;
//using System.Collections;
//using Modl;
//using System.Diagnostics;
//using Modl.Db;

//namespace Tests
//{
//    
//    public class SqlServer
//    {
//        //string databaseName = "SqlServerDb";
//        Basics basics = new Basics();

//        [TestCleanup]
//        public void Cleanup()
//        {
//            Database.DisposeAll();
//        }

//        //[Fact]
//        //public void PerformanceTest()
//        //{
//        //    int iterations = 100;
//        //    GlobalCRUD();
//        //    //basics.TestPerformance(databaseName, iterations, CacheLevel.Off, basics.CRUD);
//        //    basics.TestPerformance(databaseName, iterations, CacheLevel.On, basics.CRUD);
//        //    //basics.TestPerformance(databaseName, iterations, CacheLevel.Off, basics.CRUDExplicitId);
//        //    basics.TestPerformance(databaseName, iterations, CacheLevel.On, basics.CRUDExplicitId);
//        //}

//        ////[Fact]
//        //public void AsyncPerformanceTest()
//        //{
//        //    //GlobalCRUD();
//        //    var watch = Stopwatch.StartNew();
//        //    //basics.AsyncPerformanceCRUD(databaseName, 100, CacheLevel.Off, 1000);
//        //    //Console.WriteLine(string.Format("Total time: {0}", watch.Elapsed.TotalMilliseconds));
//        //    //watch.Restart();
//        //    basics.AsyncPerformanceCRUD(databaseName, 100, CacheLevel.On, 1000);
//        //    //Database.Get(databaseName).Dispose();
//        //    watch.Stop();
//        //    Console.WriteLine(string.Format("[{0}]Total time: {1}", databaseName, watch.Elapsed.TotalMilliseconds));
//        //}




//        //[Fact]
//        //public void GlobalCRUD()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.CRUD();
//        //}

//        //[Fact]
//        //public void StaticCRUD()
//        //{
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.SwitchStaticDatabaseAndCRUD(databaseName);
//        //}

//        //[Fact]
//        //public void InstanceCRUD()
//        //{
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.SwitchInstanceDatabaseAndCRUD(databaseName);
//        //}

//        //[Fact]
//        //public void GetFromDatabaseProvider()
//        //{
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.GetFromDatabaseProvider(databaseName);
//        //}

//        //[Fact]
//        //public void GetFromLinq()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.GetFromLinq();
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.GetFromLinqInstance(databaseName);
//        //}

//        //[Fact]
//        //public void StaticDelete()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.StaticDelete();
//        //}

//        //[Fact]
//        //public void SetIdExplicit()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.SetIdExplicit();
//        //}

//        //[Fact]
//        //public void CRUDIdExplicit()
//        //{
//        //    basics.CRUDExplicitId(Database.Get(databaseName));
//        //}

//        //[Fact]
//        //public void GetAllAsync()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.GetAllAsync();
//        //}
//    }
//}
