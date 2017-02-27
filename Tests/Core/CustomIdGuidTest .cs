using System;
using Xunit;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    
    public class CustomIdGuidTest
    {
        public class CustomIdClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public Guid CustomId { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        
        public CustomIdGuidTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CheckDefinitions()
        {
            var definitions = Modl<CustomIdClass>.Definitions;
            Assert.True(definitions.HasIdProperty);
            Assert.False(definitions.HasAutomaticId);
            Assert.Equal(1, definitions.Properties.Count);
            Assert.True(definitions.IdProperty.IsId);
            Assert.False(definitions.IdProperty.IsAutomaticId);
            Assert.False(definitions.IdProperty.IsLink);
            Assert.Equal("CustomId", definitions.IdProperty.PropertyName);
            Assert.Equal(typeof(Guid), definitions.IdProperty.PropertyType);
            Assert.Equal("CustomId", definitions.IdProperty.StorageName);
        }

        [Fact]
        public void CreateNew()
        {
            var testClass = new CustomIdClass();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            testClass = Modl<CustomIdClass>.New();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            var id = Guid.NewGuid();
            testClass = Modl<CustomIdClass>.New(id);
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.True(id == testClass.Id());
            Assert.Equal(id, testClass.CustomId);
        }

        [Fact]
        public void SetId()
        {
            var id = Guid.NewGuid();
            var testClass = new CustomIdClass();
            Assert.Equal(Guid.Empty, testClass.CustomId);
            Assert.False(testClass.Id().IsSet);
            testClass.Id(id);
            Assert.True(testClass.Id().IsSet);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);

            id = Guid.NewGuid();
            testClass.Id(id);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);

            id = Guid.NewGuid();
            testClass = new CustomIdClass();
            Assert.Equal(Guid.Empty, testClass.CustomId);
            Assert.False(testClass.Id().IsSet);
            testClass.CustomId = id;
            Assert.True(testClass.Id().IsSet);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);
            Assert.Throws<InvalidIdException>(() => testClass.Id(1));
            Assert.Throws<InvalidIdException>(() => testClass.Id("1"));
        }

        //[Fact]
        //public void GenerateId()
        //{
        //    var testClass = new CustomIdClass();
        //    var id = testClass.Id().Get();
        //    Assert.Equal(Guid.Empty, testClass.CustomId);
        //    Assert.False(testClass.Id().IsSet);
        //    Assert.True(id is Guid);
        //    Assert.Equal(id, testClass.CustomId);

        //    //testClass.Id().Generate();
        //    Assert.True(testClass.Id().IsSet);
        //    Assert.AreNotEqual(id, testClass.Id());
        //    Assert.IsNotNull(testClass.Id());
        //    Assert.True(testClass.Id().Get() is Guid);
        //    Assert.AreNotEqual(Guid.Empty, testClass.Id());
        //    Assert.Equal(testClass.Id(), testClass.CustomId);

        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());
        //}

        [Fact]
        public void Save()
        {
            var testClass = new CustomIdClass();

            Assert.Throws<InvalidIdException>(() => testClass.Save());

            var id = Guid.NewGuid();
            testClass.CustomId = id;
            testClass.Save();

            Assert.False(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.Id().Get());
            Assert.Equal(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdClass>.Get(id);
            Assert.Equal(id, loadedTestClass.CustomId);
            Assert.Equal(id, loadedTestClass.Id().Get<Guid>());
            Assert.Equal(id, loadedTestClass.CustomId);
            Assert.False(loadedTestClass.IsNew());
            Assert.False(loadedTestClass.IsModified());

            Assert.Throws<InvalidIdException>(() => loadedTestClass.Id(Guid.NewGuid()));
            Assert.Throws<InvalidIdException>(() =>
            {
                loadedTestClass.CustomId = Guid.NewGuid();
                loadedTestClass.Save();
            });
        }

        [Fact]
        public void Delete()
        {
            var testClass = new CustomIdClass();

            var id = Guid.NewGuid();
            testClass.CustomId = id;
            Assert.Throws<NotFoundException>(() => testClass.Delete());

            testClass.Save();
            Assert.False(testClass.IsDeleted());
            testClass.Delete();
            Assert.True(testClass.IsDeleted());
            Assert.Throws<NotFoundException>(() => testClass.Save());
            Assert.Throws<NotFoundException>(() => testClass.Delete());
        }
    }
}
