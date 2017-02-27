using System;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;
using Xunit;

namespace Tests.Core
{
    public class AutomaticIdTest
    {
        public class AutomaticIdGuidClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id(automatic: true)]
            public Guid CustomId { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        public class AutomaticIdIntClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id(automatic: true)]
            public int CustomId { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        public class AutomaticIdStringClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id(automatic: true)]
            public string CustomId { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        public AutomaticIdTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CheckDefinitions()
        {
            var definitions = Modl<AutomaticIdGuidClass>.Definitions;

            Assert.True(definitions.HasIdProperty);
            Assert.True(definitions.HasAutomaticId);
            Assert.Equal(1, definitions.Properties.Count);
            Assert.True(definitions.IdProperty.IsId);
            Assert.True(definitions.IdProperty.IsAutomaticId);
            Assert.False(definitions.IdProperty.IsLink);
            Assert.Equal("CustomId", definitions.IdProperty.PropertyName);
            Assert.Equal(typeof(Guid), definitions.IdProperty.PropertyType);
            Assert.Equal("CustomId", definitions.IdProperty.StorageName);
            Assert.Throws<InvalidIdException>(() => Modl<AutomaticIdIntClass>.Definitions);
            Assert.Throws<InvalidIdException>(() => Modl<AutomaticIdStringClass>.Definitions);
            //try
            //{
            //    definitions = Modl<AutomaticIdIntClass>.Definitions;
            //    Assert.Fail();
            //}
            //catch (InvalidIdException) { }

            //try
            //{
            //    definitions = Modl<AutomaticIdStringClass>.Definitions;
            //    Assert.Fail();
            //}
            //catch (InvalidIdException) { }
        }

        [Fact]
        public void CreateNew()
        {
            var testClass = new AutomaticIdGuidClass();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            testClass = Modl<AutomaticIdGuidClass>.New();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            var id = Guid.NewGuid();
            testClass = Modl<AutomaticIdGuidClass>.New(id);
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.True(testClass.Id().Equals(id));
            Assert.True(id == testClass.Id());
            Assert.Equal(id, testClass.CustomId);
        }

        [Fact]
        public void SetId()
        {
            var id = Guid.NewGuid();
            var testClass = new AutomaticIdGuidClass();
            testClass.Id(id);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);
        }

        //[Fact]
        //public void GenerateId()
        //{
        //    var testClass = new AutomaticIdGuidClass();
        //    var id = testClass.Id().Get();
        //    Assert.IsNotNull(id);
        //    Assert.True(id is Guid);
        //    Assert.AreNotEqual(Guid.Empty, id);
        //    Assert.Equal(id, testClass.CustomId);

        //    testClass.Id().Generate();
        //    Assert.True(id != testClass.Id());
        //    Assert.IsNotNull(testClass.Id().Get());
        //    Assert.True(testClass.Id().Get() is Guid);
        //    Assert.AreNotEqual(Guid.Empty, testClass.Id().Get());
        //    Assert.Equal(testClass.Id().Get(), testClass.CustomId);
        //    Assert.True(testClass.Id() == testClass.CustomId);

        //    Assert.True(testClass.IsNew());
        //    Assert.False(testClass.IsModified());
        //}

        [Fact]
        public void Save()
        {
            var testClass = new AutomaticIdGuidClass();
            var id = testClass.Id();
            testClass.Save();
            Assert.False(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.Id());
            Assert.True(id == testClass.CustomId);

            var loadedTestClass = Modl<AutomaticIdGuidClass>.Get(id);
            Assert.True(id == loadedTestClass.CustomId);
            Assert.Equal(id, loadedTestClass.Id());
            Assert.True(id == loadedTestClass.CustomId);
            Assert.False(loadedTestClass.IsNew());
            Assert.False(loadedTestClass.IsModified());
            Assert.Throws<InvalidIdException>(() => loadedTestClass.Id(Guid.NewGuid()));
        }

        [Fact]
        public void Delete()
        {
            var testClass = new AutomaticIdGuidClass();

            Assert.Throws<NotFoundException>(() => testClass.Delete());
            testClass.Save();
            Assert.False(testClass.IsDeleted());
            testClass.Delete();
            Assert.True(testClass.IsDeleted());
            Assert.Throws<NotFoundException>(() => testClass.Delete());
        }
    }
}
