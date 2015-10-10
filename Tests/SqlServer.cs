//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using ExampleModel;
//using System.Collections;
//using Modl;
//using System.Diagnostics;
//using Modl.Db;

//namespace Tests
//{
//    [TestClass]
//    public class SqlServer
//    {
//        //string databaseName = "SqlServerDb";
//        Basics basics = new Basics();

//        [TestCleanup]
//        public void Cleanup()
//        {
//            Database.DisposeAll();
//        }

//        //[TestMethod]
//        //public void PerformanceTest()
//        //{
//        //    int iterations = 100;
//        //    GlobalCRUD();
//        //    //basics.TestPerformance(databaseName, iterations, CacheLevel.Off, basics.CRUD);
//        //    basics.TestPerformance(databaseName, iterations, CacheLevel.On, basics.CRUD);
//        //    //basics.TestPerformance(databaseName, iterations, CacheLevel.Off, basics.CRUDExplicitId);
//        //    basics.TestPerformance(databaseName, iterations, CacheLevel.On, basics.CRUDExplicitId);
//        //}

//        ////[TestMethod]
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




//        //[TestMethod]
//        //public void GlobalCRUD()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.CRUD();
//        //}

//        //[TestMethod]
//        //public void StaticCRUD()
//        //{
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.SwitchStaticDatabaseAndCRUD(databaseName);
//        //}

//        //[TestMethod]
//        //public void InstanceCRUD()
//        //{
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.SwitchInstanceDatabaseAndCRUD(databaseName);
//        //}

//        //[TestMethod]
//        //public void GetFromDatabaseProvider()
//        //{
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.GetFromDatabaseProvider(databaseName);
//        //}

//        //[TestMethod]
//        //public void GetFromLinq()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.GetFromLinq();
//        //    basics.SwitchDatabase("SqlServerCeDb");
//        //    basics.GetFromLinqInstance(databaseName);
//        //}

//        //[TestMethod]
//        //public void StaticDelete()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.StaticDelete();
//        //}

//        //[TestMethod]
//        //public void SetIdExplicit()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.SetIdExplicit();
//        //}

//        //[TestMethod]
//        //public void CRUDIdExplicit()
//        //{
//        //    basics.CRUDExplicitId(Database.Get(databaseName));
//        //}

//        //[TestMethod]
//        //public void GetAllAsync()
//        //{
//        //    basics.SwitchDatabase(databaseName);
//        //    basics.GetAllAsync();
//        //}
//    }
//}
