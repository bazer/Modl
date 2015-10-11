using ExampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Json;
using Modl.Plugins;
using System;
using System.Collections.Generic;
using Modl.Structure;
using Modl.Structure.Metadata;
using System.IO;

namespace Tests.Core
{
    public class EmptyClass : IModl
    {
        public IModlData Modl { get; set; }
    }

    [TestClass]
    public class EmptyClassTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl();
        }

        [TestMethod]
        public void CheckDefinitions()
        {
            var definitions = Modl<EmptyClass>.Definitions;

            Assert.IsFalse(definitions.HasIdProperty);
            Assert.IsTrue(definitions.HasAutomaticId);
            Assert.AreEqual(0, definitions.Properties.Count);
            Assert.IsNull(definitions.IdProperty);
        }

        [TestMethod]
        public void CreateNew()
        {
            var testClass = new EmptyClass();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            testClass = Modl<EmptyClass>.New();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            var id = Guid.NewGuid();
            testClass = Modl<EmptyClass>.New(id);
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.GetId());
        }

        [TestMethod]
        public void SetId()
        {
            var id = Guid.NewGuid();
            var testClass = new EmptyClass();
            testClass.SetId(id);
            Assert.AreEqual(id, testClass.GetId());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
        }

        [TestMethod]
        public void GenerateId()
        {
            var testClass = new EmptyClass();
            var id = testClass.GetId();
            Assert.IsNotNull(id);
            Assert.IsTrue(id is Guid);
            Assert.AreNotEqual(Guid.Empty, id);
            
            testClass.GenerateId();
            Assert.AreNotEqual(id, testClass.GetId());
            Assert.IsNotNull(testClass.GetId());
            Assert.IsTrue(testClass.GetId() is Guid);
            Assert.AreNotEqual(Guid.Empty, testClass.GetId());

            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
        }

        [TestMethod]
        public void Save()
        {
            var testClass = new EmptyClass();
            var id = testClass.GetId();
            testClass.Save();
            Assert.IsFalse(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.GetId());

            var loadedTestClass = Modl<EmptyClass>.Get(id);
            Assert.AreEqual(id, loadedTestClass.GetId());
            Assert.IsFalse(loadedTestClass.IsNew());
            Assert.IsFalse(loadedTestClass.IsModified());
        }

        [TestMethod]
        public void Delete()
        {
            var testClass = new EmptyClass();

            try
            {
                testClass.Delete();
                Assert.Fail();
            }
            catch (Exception) { }

            testClass.Save();
            Assert.IsFalse(testClass.IsDeleted());
            testClass.Delete();
            Assert.IsTrue(testClass.IsDeleted());

            try
            {
                testClass.Delete();
                Assert.Fail();
            }
            catch (Exception) { }
        }
    }
}
