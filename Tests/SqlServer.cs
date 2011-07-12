using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExampleModel;

namespace Tests
{
    [TestClass]
    public class SqlServer
    {
        Basics basics = new Basics();

        [TestMethod]
        public void CRUD()
        {
            basics.SwitchDatabase("SqlServerDb");
            basics.CRUD();
        }
    }
}
