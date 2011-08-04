﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExampleModel;
using System.Collections;

namespace Tests
{
    [TestClass]
    public class SqlServer
    {
        string databaseName = "SqlServerDb";
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
            basics.SwitchDatabase("SqlServerCeDb");
            basics.SwitchStaticDatabaseAndCRUD(databaseName);
        }

        [TestMethod]
        public void InstanceCRUD()
        {
            basics.SwitchDatabase("SqlServerCeDb");
            basics.SwitchInstanceDatabaseAndCRUD(databaseName);
        }

        [TestMethod]
        public void GetFromDatabaseProvider()
        {
            basics.SwitchDatabase("SqlServerCeDb");
            basics.GetFromDatabaseProvider(databaseName);
        }

        [TestMethod]
        public void GetFromLinq()
        {
            basics.SwitchDatabase(databaseName);
            basics.GetFromLinq();
            basics.SwitchDatabase("SqlServerCeDb");
            basics.GetFromLinqInstance(databaseName);
        }

        [TestMethod]
        public void StaticDelete()
        {
            basics.SwitchDatabase(databaseName);
            basics.StaticDelete();
        }
    }
}
