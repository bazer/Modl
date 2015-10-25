﻿using System;
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
            Assert.AreEqual(id, (modl as EmptyClass).GetId());
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
            Assert.AreEqual(id, modl.GetId());
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
            Assert.IsTrue(modlList.Any(x => (Guid)x == (Guid)modl.GetId()));
            Assert.IsTrue(modlList.Any(x => (Guid)x == (Guid)modl2.GetId()));

            var modlList2 = ModlReflect.List<Guid>(typeof(EmptyClass)).ToList();
            Assert.AreNotEqual(0, modlList2.Count);
            Assert.IsTrue(modlList2.Any(x => x == (Guid)modl.GetId()));
            Assert.IsTrue(modlList2.Any(x => x == (Guid)modl2.GetId()));
        }
    }
}
