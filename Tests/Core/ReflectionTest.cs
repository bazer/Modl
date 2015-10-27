using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    [TestClass]
    public class ReflectionTest
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
        public void CreateNew()
        {
            var modl = ModlReflect.New(typeof(EmptyClass));
            Assert.AreEqual(typeof(EmptyClass), modl.GetType());
            Assert.IsTrue((modl as EmptyClass).IsNew());

            var id = Guid.NewGuid();
            modl = ModlReflect.New(typeof(EmptyClass), id);
            Assert.AreEqual(typeof(EmptyClass), modl.GetType());
            Assert.IsTrue((modl as EmptyClass).IsNew());
            Assert.IsTrue(id == (modl as EmptyClass).Id());
        }

        [TestMethod]
        public void Get()
        {
            var id = Guid.NewGuid();
            var modl = ModlReflect.New(typeof(EmptyClass), id) as EmptyClass;
            modl.Save();

            modl = ModlReflect.Get(typeof(EmptyClass), id) as EmptyClass;
            Assert.AreEqual(typeof(EmptyClass), modl.GetType());
            Assert.IsFalse(modl.IsNew());
            Assert.AreEqual(id, modl.Id().Get<Guid>());
        }

        [TestMethod]
        public void GetAll()
        {
            foreach (var m in ModlReflect.GetAll(typeof(EmptyClass)).Select(x => x as EmptyClass))
                m.Delete();

            var modlList = ModlReflect.GetAll(typeof(EmptyClass)).ToList();
            Assert.AreEqual(0, modlList.Count);

            var modl = new EmptyClass().Save();
            var modl2 = new EmptyClass().Save();

            var modlList2 = ModlReflect.GetAll(typeof(EmptyClass)).Select(x => x as EmptyClass).ToList();
            Assert.AreEqual(2, modlList2.Count);
            Assert.IsTrue(modlList2.Any(x => x.Id() == modl.Id()));
            Assert.IsTrue(modlList2.Any(x => x.Id() == modl2.Id()));
        }

        [TestMethod]
        public void List()
        {
            var modl = ModlReflect.New(typeof(EmptyClass)) as EmptyClass;
            modl.Save();

            var modl2 = ModlReflect.New(typeof(EmptyClass)) as EmptyClass;
            modl2.Save();

            var modlList = ModlReflect.List(typeof(EmptyClass)).ToList();
            Assert.AreNotEqual(0, modlList.Count);
            Assert.IsTrue(modlList.Any(x => (Guid)x == modl.Id().Get<Guid>()));
            Assert.IsTrue(modlList.Any(x => (Guid)x == modl2.Id().Get<Guid>()));

            var modlList2 = ModlReflect.List<Guid>(typeof(EmptyClass)).ToList();
            Assert.AreNotEqual(0, modlList2.Count);
            Assert.IsTrue(modlList2.Any(x => x == modl.Id().Get<Guid>()));
            Assert.IsTrue(modlList2.Any(x => x == modl2.Id().Get<Guid>()));
        }
    }
}
