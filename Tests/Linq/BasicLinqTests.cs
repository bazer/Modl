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
        public void List()
        {
            var modl = new EmptyClass().Save();
            var modl2 = new EmptyClass().Save();

            var modlList = Modl<EmptyClass>.List().ToList();
            Assert.AreNotEqual(0, modlList.Count);
            Assert.IsTrue(modlList.Any(x => (Guid)x == (Guid)modl.GetId()));
            Assert.IsTrue(modlList.Any(x => (Guid)x == (Guid)modl2.GetId()));

            var modlList2 = Modl<EmptyClass>.List<Guid>().ToList();
            Assert.AreNotEqual(0, modlList2.Count);
            Assert.IsTrue(modlList2.Any(x => x == (Guid)modl.GetId()));
            Assert.IsTrue(modlList2.Any(x => x == (Guid)modl2.GetId()));
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
