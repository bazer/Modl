using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    [TestClass]
    public class CustomIdStringTest
    {
        public class CustomIdClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public string CustomId { get; set; }
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
            Assert.AreEqual(typeof(string), definitions.IdProperty.PropertyType);
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

            var id = Guid.NewGuid().ToString();
            testClass = Modl<CustomIdClass>.New(id);
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.IsTrue(id == testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);
        }


        [TestMethod]
        public void SetId()
        {
            var id = Guid.NewGuid().ToString();
            var testClass = new CustomIdClass();
            Assert.AreEqual(null, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            testClass.Id().Set(id);
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.IsTrue(id == testClass.Id());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);

            id = Guid.NewGuid().ToString();
            testClass = new CustomIdClass();
            Assert.AreEqual(null, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            testClass.CustomId = id;
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);


            try
            {
                testClass.Id().Set(Guid.NewGuid());
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                testClass.Id().Set(1);
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void GenerateId()
        {
            var testClass = new CustomIdClass();

            try
            {
                testClass.Id().Generate();
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

            var id = Guid.NewGuid().ToString();
            testClass.CustomId = id;
            testClass.Save();

            Assert.IsFalse(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.IsTrue(id == testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdClass>.Get(id);
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsTrue(id == loadedTestClass.Id());
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsFalse(loadedTestClass.IsNew());
            Assert.IsFalse(loadedTestClass.IsModified());

            try
            {
                loadedTestClass.Id().Set(Guid.NewGuid().ToString());
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                loadedTestClass.CustomId = Guid.NewGuid().ToString();
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
            testClass.CustomId = Guid.NewGuid().ToString();

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


        [TestMethod]
        public void List()
        {
            var modl = new CustomIdClass().Id(Guid.NewGuid().ToString()).Save();
            var modl2 = new CustomIdClass().Id(Guid.NewGuid().ToString()).Save();

            var modlList = Modl<CustomIdClass>.List().ToList();
            Assert.AreNotEqual(0, modlList.Count);
            Assert.IsTrue(modlList.Any(x => x as string == modl.Id().Get<string>()));
            Assert.IsTrue(modlList.Any(x => x as string == modl2.Id().Get<string>()));

            var modlList2 = Modl<CustomIdClass>.List<string>().ToList();
            Assert.AreNotEqual(0, modlList2.Count);
            Assert.IsTrue(modlList2.Any(x => x == modl.Id().Get<string>()));
            Assert.IsTrue(modlList2.Any(x => x == modl2.Id().Get<string>()));
        }

        [TestMethod]
        public void GetAll()
        {
            foreach (var m in Modl<CustomIdClass>.GetAll())
                m.Delete();

            var modlList = Modl<CustomIdClass>.GetAll().ToList();
            Assert.AreEqual(0, modlList.Count);

            var modl = new CustomIdClass().Id(Guid.NewGuid().ToString()).Save();
            var modl2 = new CustomIdClass().Id(Guid.NewGuid().ToString()).Save();

            modlList = Modl<CustomIdClass>.GetAll().ToList();
            Assert.AreEqual(2, modlList.Count);
            Assert.IsTrue(modlList.Any(x => x.Id() == modl.Id()));
            Assert.IsTrue(modlList.Any(x => x.Id() == modl2.Id()));
        }
    }
}
