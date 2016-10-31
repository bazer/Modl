using System;
using System.Linq;
using Xunit;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    
    public class CustomIdStringTest
    {
        public class CustomIdStringClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public string CustomId { get; set; }
        }

        
        public CustomIdStringTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CheckDefinitions()
        {
            var definitions = Modl<CustomIdStringClass>.Definitions;
            Assert.True(definitions.HasIdProperty);
            Assert.False(definitions.HasAutomaticId);
            Assert.Equal(1, definitions.Properties.Count);
            Assert.True(definitions.IdProperty.IsId);
            Assert.False(definitions.IdProperty.IsAutomaticId);
            Assert.False(definitions.IdProperty.IsLink);
            Assert.Equal("CustomId", definitions.IdProperty.PropertyName);
            Assert.Equal(typeof(string), definitions.IdProperty.PropertyType);
            Assert.Equal("CustomId", definitions.IdProperty.StorageName);
        }

        [Fact]
        public void CreateNew()
        {
            var testClass = new CustomIdStringClass();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            testClass = Modl<CustomIdStringClass>.New();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            var id = Guid.NewGuid().ToString();
            testClass = Modl<CustomIdStringClass>.New(id);
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.True(id == testClass.Id());
            Assert.Equal(id, testClass.CustomId);
        }


        [Fact]
        public void SetId()
        {
            var id = Guid.NewGuid().ToString();
            var testClass = new CustomIdStringClass();
            Assert.Equal(null, testClass.CustomId);
            Assert.False(testClass.Id().IsSet);
            testClass.Id(id);
            Assert.True(testClass.Id().IsSet);
            Assert.True(id == testClass.Id());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);

            id = Guid.NewGuid().ToString();
            testClass = new CustomIdStringClass();
            Assert.Equal(null, testClass.CustomId);
            Assert.False(testClass.Id().IsSet);
            testClass.CustomId = id;
            Assert.True(testClass.Id().IsSet);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);


            try
            {
                testClass.Id(Guid.NewGuid());
                //Assert.Fail();
            }
            catch (InvalidIdException) { }

            try
            {
                testClass.Id(1);
            }
            catch (InvalidIdException) { }
        }

        //[Fact]
        //public void GenerateId()
        //{
        //    var testClass = new CustomIdClass();

        //    try
        //    {
        //        //testClass.Id().Generate();
        //        Assert.Fail();
        //    }
        //    catch (InvalidIdException) { }

        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());
        //}

        [Fact]
        public void Save()
        {
            var testClass = new CustomIdStringClass();
            Assert.Throws<InvalidIdException>(() => testClass.Save());

            var id = Guid.NewGuid().ToString();
            testClass.CustomId = id;
            testClass.Save();

            Assert.False(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.True(id == testClass.Id());
            Assert.Equal(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdStringClass>.Get(id);
            Assert.Equal(id, loadedTestClass.CustomId);
            Assert.True(id == loadedTestClass.Id());
            Assert.Equal(id, loadedTestClass.CustomId);
            Assert.False(loadedTestClass.IsNew());
            Assert.False(loadedTestClass.IsModified());
            Assert.Throws<InvalidIdException>(() => loadedTestClass.Id(Guid.NewGuid().ToString()));
            Assert.Throws<InvalidIdException>(() => 
            {
                loadedTestClass.CustomId = Guid.NewGuid().ToString();
                loadedTestClass.Save();
            });
        }

        [Fact]
        public void Delete()
        {
            var testClass = new CustomIdStringClass();

            var id = Guid.NewGuid();
            testClass.CustomId = Guid.NewGuid().ToString();
            Assert.Throws<NotFoundException>(() => testClass.Delete());

            testClass.Save();
            Assert.False(testClass.IsDeleted());
            testClass.Delete();
            Assert.True(testClass.IsDeleted());
            Assert.Throws<NotFoundException>(() => testClass.Save());
            Assert.Throws<NotFoundException>(() => testClass.Delete());
        }


        [Fact]
        public void List()
        {
            var modl = new CustomIdStringClass().Id(Guid.NewGuid().ToString()).Save();
            var modl2 = new CustomIdStringClass().Id(Guid.NewGuid().ToString()).Save();

            var modlList = Modl<CustomIdStringClass>.List().ToList();
            Assert.NotEqual(0, modlList.Count);
            Assert.True(modlList.Any(x => x as string == modl.Id().Get<string>()));
            Assert.True(modlList.Any(x => x as string == modl2.Id().Get<string>()));

            var modlList2 = Modl<CustomIdStringClass>.List<string>().ToList();
            Assert.NotEqual(0, modlList2.Count);
            Assert.True(modlList2.Any(x => x == modl.Id().Get<string>()));
            Assert.True(modlList2.Any(x => x == modl2.Id().Get<string>()));
        }

        [Fact]
        public void GetAll()
        {
            foreach (var m in Modl<CustomIdStringClass>.GetAll())
                m.Delete();

            var modlList = Modl<CustomIdStringClass>.GetAll().ToList();
            Assert.Equal(0, modlList.Count);

            var modl = new CustomIdStringClass().Id(Guid.NewGuid().ToString()).Save();
            var modl2 = new CustomIdStringClass().Id(Guid.NewGuid().ToString()).Save();

            modlList = Modl<CustomIdStringClass>.GetAll().ToList();
            Assert.Equal(2, modlList.Count);
            Assert.True(modlList.Any(x => x.Id() == modl.Id()));
            Assert.True(modlList.Any(x => x.Id() == modl2.Id()));
        }
    }
}
