﻿/*
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
