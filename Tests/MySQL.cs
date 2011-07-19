using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExampleModel;

namespace Tests
{
    [TestClass]
    public class MySQL
    {
        string databaseName = "MySQLDb";
        Basics basics = new Basics();

        //[TestMethod]
        public void PerformanceTest()
        {
            basics.PerformanceCRUD(databaseName, 100);
        }

        [TestMethod]
        public void GlobalCRUD()
        {
            basics.SwitchDatabase(databaseName);
            basics.CRUD();
        }

        [TestMethod]
        public void StaticCRUD()
        {
            basics.SwitchDatabase("SqlServerDb");
            basics.SwitchStaticDatabaseAndCRUD(databaseName);
        }

        [TestMethod]
        public void InstanceCRUD()
        {
            basics.SwitchDatabase("SqlServerDb");
            basics.SwitchInstanceDatabaseAndCRUD(databaseName);
        }

        [TestMethod]
        public void GetFromDatabaseProvider()
        {
            basics.SwitchDatabase("SqlServerDb");
            basics.GetFromDatabaseProvider(databaseName);
        }

        [TestMethod]
        public void GetFromLinq()
        {
            basics.SwitchDatabase(databaseName);
            basics.GetFromLinq();
            basics.SwitchDatabase("SqlServerDb");
            basics.GetFromLinqInstance(databaseName);
        }
    }
}
