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
        public class CustomIdIntClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public int CustomId { get; set; }
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var obj in Modl<CustomIdIntClass>.GetAll())
                obj.Delete();
        }

        [TestInitialize]
        public void Initialize()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [TestMethod]
        public void CheckDefinitions()
        {
            var definitions = Modl<CustomIdIntClass>.Definitions;
            Assert.IsTrue(definitions.HasIdProperty);
            Assert.IsFalse(definitions.HasAutomaticId);
            Assert.AreEqual(1, definitions.Properties.Count);
            Assert.IsTrue(definitions.IdProperty.IsId);
            Assert.IsFalse(definitions.IdProperty.IsAutomaticId);
            Assert.IsFalse(definitions.IdProperty.IsLink);
            Assert.AreEqual("CustomId", definitions.IdProperty.PropertyName);
            Assert.AreEqual(typeof(int), definitions.IdProperty.PropertyType);
            Assert.AreEqual("CustomId", definitions.IdProperty.StorageName);
        }

        [TestMethod]
        public void CreateNew()
        {
            var testClass = new CustomIdIntClass();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            testClass = Modl<CustomIdIntClass>.New();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            var id = 3433;
            testClass = Modl<CustomIdIntClass>.New(id);
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.IsTrue(id == testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);
        }


        [TestMethod]
        public void SetId()
        {
            var id = 644564;
            var testClass = new CustomIdIntClass();
            Assert.AreEqual(0, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            testClass.Id(id);
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);

            id = 747474;
            testClass = new CustomIdIntClass();
            Assert.AreEqual(0, testClass.CustomId);
            Assert.IsFalse(testClass.Id().IsSet);
            testClass.CustomId = id;
            Assert.IsTrue(testClass.Id().IsSet);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);


            try
            {
                testClass.Id(Guid.NewGuid());
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                testClass.Id("644563434");
            }
            catch (InvalidIdException) { }
        }

        //[TestMethod]
        //public void GenerateId()
        //{
        //    var testClass = new CustomIdClass();

        //    try
        //    {
        //        //testClass.Id().Generate();
        //        Assert.Fail();
        //    }
        //    catch (InvalidIdException) { }
            
        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());
        //}

        [TestMethod]
        public void Save()
        {
            var testClass = new CustomIdIntClass();

            try
            {
                testClass.Save();
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            var id = 2472;
            testClass.CustomId = id;
            testClass.Save();

            Assert.IsFalse(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.IsTrue(id == testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdIntClass>.Get(id);
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsTrue(id == loadedTestClass.Id());
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsFalse(loadedTestClass.IsNew());
            Assert.IsFalse(loadedTestClass.IsModified());

            try
            {
                loadedTestClass.Id(4544);
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                loadedTestClass.CustomId = 4544;
                loadedTestClass.Save();
                Assert.Fail();
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void Delete()
        {
            var testClass = new CustomIdIntClass();

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
