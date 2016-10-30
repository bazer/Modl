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

            [Name("Class2-relation")]
            public ModlCollection<Class2> MultipleRelation { get; set; }
        }

        public class Class2: IModl
        {
            public IModlData Modl { get; set; }
            [Name("Class1-relation")]
            public ModlValue<Class1> SingleRelation { get; set; }
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
            var definitions1 = Modl<Class1>.Definitions;
            Assert.AreEqual(1, definitions1.Properties.Count);
            Assert.IsTrue(definitions1.Properties.First().IsLink);

            var definitions2 = Modl<Class2>.Definitions;
            Assert.AreEqual(1, definitions2.Properties.Count);
            Assert.IsTrue(definitions2.Properties.First().IsLink);
        }

        [TestMethod]
        public void AddMultipleRelations()
        {
            var class1 = new Class1().Modl();
            class1.MultipleRelation.Add(new Class2());
            class1.MultipleRelation.Add(new Class2());
            class1.Save();
            foreach (var class2 in class1.MultipleRelation)
                class2.Save();

            Assert.AreEqual(2, class1.MultipleRelation.Count());
            Assert.AreEqual(class1.Id(), class1.MultipleRelation.First().SingleRelation.Id);
            Assert.AreEqual(class1.Id(), class1.MultipleRelation.First().SingleRelation.Val.Id());


            var loadedClass1 = Modl<Class1>.Get(class1.Id());
            Assert.AreEqual(2, loadedClass1.MultipleRelation.Count());
            Assert.AreEqual(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Id);
            Assert.AreEqual(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Val.Id());

            foreach (var class2 in class1.MultipleRelation)
            {
                var loadedClass2 = Modl<Class2>.Get(class2.Id());
                Assert.IsNotNull(loadedClass2.SingleRelation.Val);
                Assert.AreEqual(2, loadedClass2.SingleRelation.Val.MultipleRelation.Count());
                Assert.IsTrue(loadedClass2.SingleRelation.Val.MultipleRelation.Any(x => x.Id() == loadedClass2.Id()));
            }
        }

        [TestMethod]
        public void AddSingleRelation()
        {
            var class2 = new Class2().Modl();
            class2.SingleRelation.Val = new Class1();
            class2.Save();
            class2.SingleRelation.Val.Save();

            Assert.IsNotNull(class2.SingleRelation.Val);
            Assert.AreEqual(1, class2.SingleRelation.Val.MultipleRelation.Count());
            Assert.AreEqual(class2.Id(), class2.SingleRelation.Val.MultipleRelation.First().Id());

            var loadedClass2 = Modl<Class2>.Get(class2.Id());
            Assert.IsNotNull(loadedClass2.SingleRelation.Val);
            Assert.AreEqual(1, loadedClass2.SingleRelation.Val.MultipleRelation.Count());
            Assert.AreEqual(loadedClass2.Id(), loadedClass2.SingleRelation.Val.MultipleRelation.First().Id());


            var loadedClass1 = Modl<Class1>.Get(class2.SingleRelation.Val.Id());
            Assert.AreEqual(1, loadedClass1.MultipleRelation.Count());
            Assert.AreEqual(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Id);
            Assert.AreEqual(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Val.Id());
        }
    }
}
