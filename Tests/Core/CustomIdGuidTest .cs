using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    [TestClass]
    public class CustomIdGuidTest
    {
        public class CustomIdClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public Guid CustomId { get; set; }
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
            Assert.IsFalse(definitions.IdProperty.IsLink);
            Assert.AreEqual("CustomId", definitions.IdProperty.PropertyName);
            Assert.AreEqual(typeof(Guid), definitions.IdProperty.PropertyType);
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

            var id = Guid.NewGuid();
            testClass = Modl<CustomIdClass>.New(id);
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.IsTrue(id == testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);
        }

        [TestMethod]
        public void SetId()
        {
            var id = Guid.NewGuid();
            var testClass = new CustomIdClass();
            Assert.AreEqual(Guid.Empty, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            testClass.Id().Set(id);
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);

            id = Guid.NewGuid();
            testClass.Id().Set(id);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);

            id = Guid.NewGuid();
            testClass = new CustomIdClass();
            Assert.AreEqual(Guid.Empty, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            testClass.CustomId = id;
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);


            try
            {
                testClass.Id().Set(1);
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                testClass.Id().Set("1");
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void GenerateId()
        {
            var testClass = new CustomIdClass();
            var id = testClass.Id().Get();
            Assert.AreEqual(Guid.Empty, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            Assert.IsTrue(id is Guid);
            Assert.AreEqual(id, testClass.CustomId);

            testClass.Id().Generate();
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.AreNotEqual(id, testClass.Id());
            Assert.IsNotNull(testClass.Id());
            Assert.IsTrue(testClass.Id().Get() is Guid);
            Assert.AreNotEqual(Guid.Empty, testClass.Id());
            Assert.AreEqual(testClass.Id(), testClass.CustomId);

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

            var id = Guid.NewGuid();
            testClass.CustomId = id;
            testClass.Save();

            Assert.IsFalse(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.AreEqual(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdClass>.Get(id);
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.AreEqual(id, loadedTestClass.Id().Get<Guid>());
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsFalse(loadedTestClass.IsNew());
            Assert.IsFalse(loadedTestClass.IsModified());

            try
            {
                loadedTestClass.Id().Set(Guid.NewGuid());
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                loadedTestClass.CustomId = Guid.NewGuid();
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
            testClass.CustomId = id;

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
