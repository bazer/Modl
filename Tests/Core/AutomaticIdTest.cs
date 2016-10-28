using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    [TestClass]
    public class AutomaticIdTest
    {
        public class AutomaticIdGuidClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id(automatic: true)]
            public Guid CustomId { get; set; }
        }

        public class AutomaticIdIntClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id(automatic: true)]
            public int CustomId { get; set; }
        }

        public class AutomaticIdStringClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id(automatic: true)]
            public string CustomId { get; set; }
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
            var definitions = Modl<AutomaticIdGuidClass>.Definitions;

            Assert.IsTrue(definitions.HasIdProperty);
            Assert.IsTrue(definitions.HasAutomaticId);
            Assert.AreEqual(1, definitions.Properties.Count);
            Assert.IsTrue(definitions.IdProperty.IsId);
            Assert.IsTrue(definitions.IdProperty.IsAutomaticId);
            Assert.IsFalse(definitions.IdProperty.IsLink);
            Assert.AreEqual("CustomId", definitions.IdProperty.PropertyName);
            Assert.AreEqual(typeof(Guid), definitions.IdProperty.PropertyType);
            Assert.AreEqual("CustomId", definitions.IdProperty.StorageName);

            try
            {
                definitions = Modl<AutomaticIdIntClass>.Definitions;
                Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                definitions = Modl<AutomaticIdStringClass>.Definitions;
                Assert.Fail();
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void CreateNew()
        {
            var testClass = new AutomaticIdGuidClass();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            testClass = Modl<AutomaticIdGuidClass>.New();
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());

            var id = Guid.NewGuid();
            testClass = Modl<AutomaticIdGuidClass>.New(id);
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.IsTrue(testClass.Id().Equals(id));
            Assert.IsTrue(id == testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);
        }

        [TestMethod]
        public void SetId()
        {
            var id = Guid.NewGuid();
            var testClass = new AutomaticIdGuidClass();
            testClass.Id(id);
            Assert.AreEqual(id, testClass.Id().Get());
            Assert.IsTrue(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.CustomId);
        }

        //[TestMethod]
        //public void GenerateId()
        //{
        //    var testClass = new AutomaticIdGuidClass();
        //    var id = testClass.Id().Get();
        //    Assert.IsNotNull(id);
        //    Assert.IsTrue(id is Guid);
        //    Assert.AreNotEqual(Guid.Empty, id);
        //    Assert.AreEqual(id, testClass.CustomId);

        //    testClass.Id().Generate();
        //    Assert.IsTrue(id != testClass.Id());
        //    Assert.IsNotNull(testClass.Id().Get());
        //    Assert.IsTrue(testClass.Id().Get() is Guid);
        //    Assert.AreNotEqual(Guid.Empty, testClass.Id().Get());
        //    Assert.AreEqual(testClass.Id().Get(), testClass.CustomId);
        //    Assert.IsTrue(testClass.Id() == testClass.CustomId);

        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());
        //}

        [TestMethod]
        public void Save()
        {
            var testClass = new AutomaticIdGuidClass();
            var id = testClass.Id();
            testClass.Save();
            Assert.IsFalse(testClass.IsNew());
            Assert.IsFalse(testClass.IsModified());
            Assert.AreEqual(id, testClass.Id());
            Assert.AreEqual(id, testClass.CustomId);

            var loadedTestClass = Modl<AutomaticIdGuidClass>.Get(id);
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.AreEqual(id, loadedTestClass.Id());
            Assert.AreEqual(id, loadedTestClass.CustomId);
            Assert.IsFalse(loadedTestClass.IsNew());
            Assert.IsFalse(loadedTestClass.IsModified());

            try
            {
                loadedTestClass.Id(Guid.NewGuid());
                Assert.Fail();
            }
            catch (InvalidIdException) { }
        }

        [TestMethod]
        public void Delete()
        {
            var testClass = new AutomaticIdGuidClass();

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
                testClass.Delete();
                Assert.Fail();
            }
            catch (NotFoundException) { }
        }
    }
}
