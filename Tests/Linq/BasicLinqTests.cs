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
            foreach (var m in Modl<EmptyClass>.Query())
                m.Delete();

            var modls = Modl<EmptyClass>.Query().ToList();
            Assert.AreEqual(0, modls.Count);

            var modl = new EmptyClass();
            modl.Save();

            modls = Modl<EmptyClass>.Query().ToList();
            Assert.AreEqual(1, modls.Count);
        }
    }
}
