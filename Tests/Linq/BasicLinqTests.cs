using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Linq
{
    [TestClass]
    public class BasicLinqTests
    {
        public class EmptyClass : IModl
        {
            public IModlData Modl { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl();
        }

        [TestMethod]
        public void GetAll()
        {
            var modls = Modl<EmptyClass>.Query().ToList();
        }
    }
}
