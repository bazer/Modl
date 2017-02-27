using System;
using Xunit;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{

    public class CustomIdIntTest: IDisposable
    {
        public class CustomIdIntClass : IModl
        {
            public IModlData Modl { get; set; }
            [Id]
            public int CustomId { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }

        //[TestCleanup]
        //public void Cleanup()
        //{
        //    foreach (var obj in Modl<CustomIdIntClass>.GetAll())
        //        obj.Delete();
        //}

        private void Cleanup(int id)
        {
            if (Modl<CustomIdIntClass>.Exists(id))
                Modl<CustomIdIntClass>.Get(id).Delete();
        }

        public CustomIdIntTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CheckDefinitions()
        {
            var definitions = Modl<CustomIdIntClass>.Definitions;
            Assert.True(definitions.HasIdProperty);
            Assert.False(definitions.HasAutomaticId);
            Assert.Equal(1, definitions.Properties.Count);
            Assert.True(definitions.IdProperty.IsId);
            Assert.False(definitions.IdProperty.IsAutomaticId);
            Assert.False(definitions.IdProperty.IsLink);
            Assert.Equal("CustomId", definitions.IdProperty.PropertyName);
            Assert.Equal(typeof(int), definitions.IdProperty.PropertyType);
            Assert.Equal("CustomId", definitions.IdProperty.StorageName);
        }

        [Fact]
        public void CreateNew()
        {


            var testClass = new CustomIdIntClass();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            testClass = Modl<CustomIdIntClass>.New();
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());

            var id = 3433;
            testClass = Modl<CustomIdIntClass>.New(id);
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.True(id == testClass.Id());
            Assert.Equal(id, testClass.CustomId);
        }


        [Fact]
        public void SetId()
        {
            var id = 644564;
            var testClass = new CustomIdIntClass();
            Assert.Equal(0, testClass.CustomId);
            Assert.False(testClass.Id().IsSet);
            testClass.Id(id);
            Assert.True(testClass.Id().IsSet);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);

            id = 747474;
            testClass = new CustomIdIntClass();
            Assert.Equal(0, testClass.CustomId);
            Assert.False(testClass.Id().IsSet);
            testClass.CustomId = id;
            Assert.True(testClass.Id().IsSet);
            Assert.Equal(id, testClass.Id().Get());
            Assert.True(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.Equal(id, testClass.CustomId);

            id = 644563434;
            testClass.Id(id.ToString());
            Assert.Equal(id, testClass.CustomId);
            Assert.True(id == testClass.Id());
            Assert.Throws<InvalidIdException>(() => testClass.Id(Guid.NewGuid()));
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
            var testClass = new CustomIdIntClass();
            Assert.Throws<InvalidIdException>(() => testClass.Save());

            var id = 2472;
            testClass.CustomId = id;
            testClass.Save();

            Assert.False(testClass.IsNew());
            Assert.False(testClass.IsModified());
            Assert.True(id == testClass.Id());
            Assert.Equal(id, testClass.CustomId);

            var loadedTestClass = Modl<CustomIdIntClass>.Get(id);
            Assert.Equal(id, loadedTestClass.CustomId);
            Assert.True(id == loadedTestClass.Id());
            Assert.Equal(id, loadedTestClass.CustomId);
            Assert.False(loadedTestClass.IsNew());
            Assert.False(loadedTestClass.IsModified());
            Assert.Throws<InvalidIdException>(() => loadedTestClass.Id(4544));
            Assert.Throws<InvalidIdException>(() =>
            {
                loadedTestClass.CustomId = 4544;
                loadedTestClass.Save();
            });
        }

        [Fact]
        public void Delete()
        {
            var testClass = new CustomIdIntClass();

            var id = Guid.NewGuid();
            testClass.CustomId = 1;
            Assert.Throws<NotFoundException>(() => testClass.Delete());

            testClass.Save();
            Assert.False(testClass.IsDeleted());
            testClass.Delete();
            Assert.True(testClass.IsDeleted());
            Assert.Throws<NotFoundException>(() => testClass.Save());
            Assert.Throws<NotFoundException>(() => testClass.Delete());
        }

        public void Dispose()
        {
            Cleanup(3433);
            Cleanup(644564);
            Cleanup(747474);
            Cleanup(644563434);
            Cleanup(2472);
            Cleanup(4544);
        }
    }
}
