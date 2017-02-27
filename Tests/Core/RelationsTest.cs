using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    
    public class RelationsTest
    {
        public class Class1 : IModl
        {
            public IModlData Modl { get; set; }

            [Name("Class2-relation")]
            public ModlCollection<Class2> MultipleRelation { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        public class Class2: IModl
        {
            public IModlData Modl { get; set; }
            [Name("Class1-relation")]
            public ModlValue<Class1> SingleRelation { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        public class Class3 : IModl
        {
            public IModlData Modl { get; set; }
            public Class1 SingleRelation { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }


        public RelationsTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CheckDefinitions()
        {
            var definitions1 = Modl<Class1>.Definitions;
            Assert.Equal(1, definitions1.Properties.Count);
            Assert.True(definitions1.Properties.First().IsLink);

            var definitions2 = Modl<Class2>.Definitions;
            Assert.Equal(1, definitions2.Properties.Count);
            Assert.True(definitions2.Properties.First().IsLink);
        }

        [Fact]
        public void AddMultipleRelations()
        {
            var class1 = new Class1().Modl();
            class1.MultipleRelation.Add(new Class2());
            class1.MultipleRelation.Add(new Class2());
            class1.Save();
            foreach (var class2 in class1.MultipleRelation)
                class2.Save();

            Assert.Equal(2, class1.MultipleRelation.Count());
            Assert.Equal(class1.Id(), class1.MultipleRelation.First().SingleRelation.Id);
            Assert.Equal(class1.Id(), class1.MultipleRelation.First().SingleRelation.Val.Id());


            var loadedClass1 = Modl<Class1>.Get(class1.Id());
            Assert.Equal(2, loadedClass1.MultipleRelation.Count());
            Assert.Equal(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Id);
            Assert.Equal(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Val.Id());

            foreach (var class2 in class1.MultipleRelation)
            {
                var loadedClass2 = Modl<Class2>.Get(class2.Id());
                Assert.NotNull(loadedClass2.SingleRelation.Val);
                Assert.Equal(2, loadedClass2.SingleRelation.Val.MultipleRelation.Count());
                Assert.True(loadedClass2.SingleRelation.Val.MultipleRelation.Any(x => x.Id() == loadedClass2.Id()));
            }
        }

        [Fact]
        public void AddSingleRelation()
        {
            var class2 = new Class2().Modl();
            class2.SingleRelation.Val = new Class1();
            class2.Save();
            class2.SingleRelation.Val.Save();

            Assert.NotNull(class2.SingleRelation.Val);
            Assert.Equal(1, class2.SingleRelation.Val.MultipleRelation.Count());
            Assert.Equal(class2.Id(), class2.SingleRelation.Val.MultipleRelation.First().Id());

            var loadedClass2 = Modl<Class2>.Get(class2.Id());
            Assert.NotNull(loadedClass2.SingleRelation.Val);
            Assert.Equal(1, loadedClass2.SingleRelation.Val.MultipleRelation.Count());
            Assert.Equal(loadedClass2.Id(), loadedClass2.SingleRelation.Val.MultipleRelation.First().Id());


            var loadedClass1 = Modl<Class1>.Get(class2.SingleRelation.Val.Id());
            Assert.Equal(1, loadedClass1.MultipleRelation.Count());
            Assert.Equal(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Id);
            Assert.Equal(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Val.Id());
        }

        [Fact]
        public void AddSingleRelationWithoutModlValue()
        {
            var class2 = new Class3().Modl();
            class2.SingleRelation = new Class1();
            class2.Save();
            class2.SingleRelation.Save();

            Assert.NotNull(class2.SingleRelation);
            Assert.Equal(1, class2.SingleRelation.MultipleRelation.Count());
            Assert.Equal(class2.Id(), class2.SingleRelation.MultipleRelation.First().Id());

            var loadedClass2 = Modl<Class3>.Get(class2.Id());
            Assert.NotNull(loadedClass2.SingleRelation);
            Assert.Equal(1, loadedClass2.SingleRelation.MultipleRelation.Count());
            Assert.Equal(loadedClass2.Id(), loadedClass2.SingleRelation.MultipleRelation.First().Id());


            var loadedClass1 = Modl<Class1>.Get(class2.SingleRelation.Id());
            Assert.Equal(1, loadedClass1.MultipleRelation.Count());
            Assert.Equal(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Id);
            Assert.Equal(loadedClass1.Id(), loadedClass1.MultipleRelation.First().SingleRelation.Val.Id());
        }
    }
}
