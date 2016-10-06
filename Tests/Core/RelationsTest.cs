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
    public class RelationsTest
    {
        public class Class1 : IModl
        {
            public IModlData Modl { get; set; }
            //public IEnumerable<Class2> MultipleRelation
            //{
            //    get { return this.Relation(nameof(MultipleRelation)).GetValue<Class2>(); }
            //    //set { this.Relation(nameof(MultipleRelation)).SetValue(value); }
            //}
        }

        public class Class2: IModl
        {
            public IModlData Modl { get; set; }
            //public Relation<Class1> SingleRelation
            //{
            //    get { return this.Relation(nameof(SingleRelation)).GetValue<Class1>(); }
            //    set { this.Relation(nameof(SingleRelation)).SetValue(value); }
            //}


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
            var definitions1 = Modl<Class1>.Definitions;
            Assert.AreEqual(1, definitions1.Properties.Count);
            Assert.IsTrue(definitions1.Properties.First().IsRelation);

            var definitions2 = Modl<Class2>.Definitions;
            Assert.AreEqual(1, definitions2.Properties.Count);
            Assert.IsTrue(definitions2.Properties.First().IsRelation);
        }

        //[TestMethod]
        //public void CreateNew()
        //{
        //    var testClass = new EmptyClass();
        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());

        //    testClass = Modl<EmptyClass>.New();
        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());

        //    var id = Guid.NewGuid();
        //    testClass = Modl<EmptyClass>.New(id);
        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());
        //    Assert.IsTrue(id == testClass.Id());
        //}

        //[TestMethod]
        //public void SetId()
        //{
        //    var id = Guid.NewGuid();
        //    var testClass = new EmptyClass();
        //    testClass.Id(id);
        //    Assert.AreEqual(id, testClass.Id().Get());
        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());
        //}

        //[TestMethod]
        //public void GenerateId()
        //{
        //    var testClass = new EmptyClass();
        //    var id = testClass.Id().Get();
        //    Assert.IsNotNull(id);
        //    Assert.IsTrue(id is Guid);
        //    Assert.AreNotEqual(Guid.Empty, id);

        //    testClass.Id().Generate();
        //    Assert.AreNotEqual(id, testClass.Id());
        //    Assert.IsNotNull(testClass.Id());
        //    Assert.IsTrue(testClass.Id().Get() is Guid);
        //    Assert.AreNotEqual(Guid.Empty, testClass.Id());

        //    Assert.IsTrue(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());
        //}

        //[TestMethod]
        //public void Save()
        //{
        //    var testClass = new WeirdClass();
        //    var id = testClass.Id();
        //    testClass.Save();
        //    Assert.IsFalse(testClass.IsNew());
        //    Assert.IsFalse(testClass.IsModified());
        //    Assert.AreEqual(id, testClass.Id());

        //    var loadedTestClass = Modl<WeirdClass>.Get(id);
        //    Assert.AreEqual(id, loadedTestClass.Id());
        //    Assert.IsFalse(loadedTestClass.IsNew());
        //    Assert.IsFalse(loadedTestClass.IsModified());
        //}

        //[TestMethod]
        //public void Delete()
        //{
        //    var testClass = new EmptyClass();

        //    try
        //    {
        //        testClass.Delete();
        //        Assert.Fail();
        //    }
        //    catch (NotFoundException) { }

        //    testClass.Save();
        //    Assert.IsFalse(testClass.IsDeleted());
        //    testClass.Delete();
        //    Assert.IsTrue(testClass.IsDeleted());

        //    try
        //    {
        //        testClass.Delete();
        //        Assert.Fail();
        //    }
        //    catch (NotFoundException) { }
        //}

        //[TestMethod]
        //public void List()
        //{
        //    var modl = new EmptyClass().Save();
        //    var modl2 = new EmptyClass().Save();

        //    var modlList = Modl<EmptyClass>.List().ToList();
        //    Assert.AreNotEqual(0, modlList.Count);
        //    Assert.IsTrue(modlList.Any(x => x == modl.Id()));
        //    Assert.IsTrue(modlList.Any(x => x == modl2.Id()));

        //    var modlList2 = Modl<EmptyClass>.List<Guid>().ToList();
        //    Assert.AreNotEqual(0, modlList2.Count);
        //    Assert.IsTrue(modlList2.Any(x => x == modl.Id()));
        //    Assert.IsTrue(modlList2.Any(x => x == modl2.Id()));
        //}

        //[TestMethod]
        //public void GetAll()
        //{
        //    foreach (var m in Modl<EmptyClass>.GetAll())
        //        m.Delete();

        //    var modlList = Modl<EmptyClass>.GetAll().ToList();
        //    Assert.AreEqual(0, modlList.Count);

        //    var modl = new EmptyClass().Save();
        //    var modl2 = new EmptyClass().Save();

        //    modlList = Modl<EmptyClass>.GetAll().ToList();
        //    Assert.AreEqual(2, modlList.Count);
        //    Assert.IsTrue(modlList.Any(x => x.Id() == modl.Id()));
        //    Assert.IsTrue(modlList.Any(x => x.Id() == modl2.Id()));
        //}
    }
}
