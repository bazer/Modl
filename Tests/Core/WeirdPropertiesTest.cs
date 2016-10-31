using System;
using System.Linq;
using Xunit;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    
    public class WeirdPropertiesTest
    {
        public abstract class Base
        {
            abstract public int Id { get; set; }
        }

        public class WeirdClass : Base, IModl
        {
            protected int id = 0;
            public override int Id { get { return id; } set { id = value; } }

            public IModlData Modl { get; set; }


        }

        
        public WeirdPropertiesTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CheckDefinitions()
        {
            var definitions = Modl<WeirdClass>.Definitions;

            Assert.False(definitions.HasIdProperty);
            Assert.True(definitions.HasAutomaticId);
            Assert.Equal(1, definitions.Properties.Count);
            Assert.Null(definitions.IdProperty);
        }

        //[Fact]
        //public void CreateNew()
        //{
        //    var testClass = new EmptyClass();
        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());

        //    testClass = Modl<EmptyClass>.New();
        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());

        //    var id = Guid.NewGuid();
        //    testClass = Modl<EmptyClass>.New(id);
        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());
        //    Assert.True(id == testClass.Id());
        //}

        //[Fact]
        //public void SetId()
        //{
        //    var id = Guid.NewGuid();
        //    var testClass = new EmptyClass();
        //    testClass.Id(id);
        //    Assert.Equal(id, testClass.Id().Get());
        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());
        //}

        //[Fact]
        //public void GenerateId()
        //{
        //    var testClass = new EmptyClass();
        //    var id = testClass.Id().Get();
        //    Assert.IsNotNull(id);
        //    Assert.True(id is Guid);
        //    Assert.AreNotEqual(Guid.Empty, id);

        //    testClass.Id().Generate();
        //    Assert.AreNotEqual(id, testClass.Id());
        //    Assert.IsNotNull(testClass.Id());
        //    Assert.True(testClass.Id().Get() is Guid);
        //    Assert.AreNotEqual(Guid.Empty, testClass.Id());

        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());
        //}

        [Fact]
        public void Save()
        {
            var testClass = new WeirdClass();
            var id = testClass.Id();
            testClass.Save();
            Assert.False(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.Id());

            var loadedTestClass = Modl<WeirdClass>.Get(id);
            Assert.Equal(id, loadedTestClass.Id());
            Assert.False(loadedTestClass.IsNew());
            Assert.False(loadedTestClass.IsModified());
        }

        //[Fact]
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
        //    Assert.False(testClass.IsDeleted());
        //    testClass.Delete();
        //    Assert.True(testClass.IsDeleted());

        //    try
        //    {
        //        testClass.Delete();
        //        Assert.Fail();
        //    }
        //    catch (NotFoundException) { }
        //}

        //[Fact]
        //public void List()
        //{
        //    var modl = new EmptyClass().Save();
        //    var modl2 = new EmptyClass().Save();

        //    var modlList = Modl<EmptyClass>.List().ToList();
        //    Assert.AreNotEqual(0, modlList.Count);
        //    Assert.True(modlList.Any(x => x == modl.Id()));
        //    Assert.True(modlList.Any(x => x == modl2.Id()));

        //    var modlList2 = Modl<EmptyClass>.List<Guid>().ToList();
        //    Assert.AreNotEqual(0, modlList2.Count);
        //    Assert.True(modlList2.Any(x => x == modl.Id()));
        //    Assert.True(modlList2.Any(x => x == modl2.Id()));
        //}

        //[Fact]
        //public void GetAll()
        //{
        //    foreach (var m in Modl<EmptyClass>.GetAll())
        //        m.Delete();

        //    var modlList = Modl<EmptyClass>.GetAll().ToList();
        //    Assert.Equal(0, modlList.Count);

        //    var modl = new EmptyClass().Save();
        //    var modl2 = new EmptyClass().Save();

        //    modlList = Modl<EmptyClass>.GetAll().ToList();
        //    Assert.Equal(2, modlList.Count);
        //    Assert.True(modlList.Any(x => x.Id() == modl.Id()));
        //    Assert.True(modlList.Any(x => x.Id() == modl2.Id()));
        //}
    }
}
