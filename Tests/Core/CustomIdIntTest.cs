using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    [TestClass]
    public class CustomIdIntTest
    {
        public class CustomIdClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public int CustomId { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl();
        }

        [TestMethod]
        public void CheckDefinitions()
        {
            var definitions = Modl<CustomIdClass>.Definitions;
            Assert.IsTrue(definitions.HasIdProperty);
            Assert.IsFalse(definitions.HasAutomaticId);
            Assert.AreEqual(1, definitions.Properties.Count);
            Assert.IsTrue(definitions.IdProperty.IsId);
            Assert.IsFalse(definitions.IdProperty.IsAutomaticId);
            Assert.IsFalse(definitions.IdProperty.IsRelation);
            Assert.AreEqual("CustomId", definitions.IdProperty.PropertyName);
            Assert.AreEqual(typeof(int), definitions.IdProperty.PropertyType);
            Assert.AreEqual("CustomId", definitions.IdProperty.StorageName);
        }

        [TestMethod]
        public void CreateNew()
        {
            var testClass = new CustomIdClass();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            testClass = Modl<CustomIdClass>.New();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            var id = 1;
            testClass = Modl<CustomIdClass>.New(id);
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.GetId());
            Assert.AreEqual(id, testClass.CustomId);
        }


        [TestMethod]
        public void SetId()
        {
            var id = 1;
            var testClass = new CustomIdClass();
            Assert.AreEqual(0, testClass.CustomId);
            Assert.IsFalse(testClass.HasId());
            testClass.SetId(id);
            Assert.IsTrue(testClass.HasId());
            Assert.AreEqual(id, testClass.GetId());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);

            id = 2;
            testClass = new CustomIdClass();
            Assert.AreEqual(0, testClass.CustomId);
            Assert.IsFalse(testClass.HasId());
            testClass.CustomId = id;
            Assert.IsTrue(testClass.HasId());
            Assert.AreEqual(id, testClass.GetId());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);


            try
            {
                testClass.SetId(Guid.NewGuid());
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                testClass.SetId("1");
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void GenerateId()
        {
            var testClass = new CustomIdClass();

            try
            {
                testClass.GenerateId();
                Assert.Fail();
            }
            catch (InvalidIdException) { }
            
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
        }

        [TestMethod]
        public void Save()
        {
            var testClass = new CustomIdClass();

            try
            {
                testClass.Save();
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            var id = 1;
            testClass.CustomId = id;
            testClass.Save();

            Assert.IsFalse(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.GetId());
            Assert.AreEqual(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdClass>.Get(id);
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.AreEqual(id, loadedTestClass.GetId());
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsFalse(loadedTestClass.IsNew());
            Assert.IsFalse(loadedTestClass.IsModified());

            try
            {
                loadedTestClass.SetId(2);
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                loadedTestClass.CustomId = 2;
                loadedTestClass.Save();
                Assert.Fail();
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void Delete()
        {
            var testClass = new CustomIdClass();

            var id = Guid.NewGuid();
            testClass.CustomId = 1;

            try
            {
                testClass.Delete();
                Assert.Fail();
            }
            catch (NotFoundException) { }

            testClass.Save();
            Assert.IsFalse(testClass.IsDeleted());
            testClass.Delete();
            Assert.IsTrue(testClass.IsDeleted());

            try
            {
                testClass.Save();
                Assert.Fail();
            }
            catch (NotFoundException) { }

            try
            {
                testClass.Delete();
                Assert.Fail();
            }
            catch (NotFoundException) { }
        }
    }
}
