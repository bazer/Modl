//using ExampleModel;
//using Modl;
//using Modl.Instance;
//using Modl.Json;
//using Modl.Plugins;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Tests.Immutable
//{
//    [Name("ImmutableTable")]
//    public interface IImmutableTable : IModl
//    {
//        int Value { get; }
//        int Value2 { get; }
//        int Value3 { get; }
//    }

//    public interface OneValue : IPartialModl<IImmutableTable>
//    {
//        int Value { get; }
//    }

//    public interface TwoValues : IPartialModl<IImmutableTable>
//    {
//        int Value { get; }
//        int Value2 { get; }
//    }



//    public struct OneValueSave : IWriteModl<IImmutableTable>, IImmutableTable
//    {
//        public int Value { get; set; }
//        public int Value2 { get; set; }
//        public int Value3 { get; set; }
//        public IModlData Modl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//    }

//    public class Basics
//    {
//        public Basics()
//        {
//            Settings.GlobalSettings.Serializer = new JsonModl();
//            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
//        }

//        //[Fact]
//        //public void CheckDefinitions()
//        //{
//        //    var definitions = ImmutableModl<SimpleInterfaceClass>.Definitions;

//        //    Assert.False(definitions.HasIdProperty);
//        //    Assert.True(definitions.HasAutomaticId);
//        //    Assert.Equal(0, definitions.Properties.Count);
//        //    Assert.Null(definitions.IdProperty);
//        //}

//        [Fact]
//        public void CreateNew()
//        {
//            var newVal = 2;

//            var car = Modl<ICar>.Get(234);
//            Assert.Equal("test", car.Name);

//            var change = car
//                //.Modify(x => ((x.Name, "test"), (x.Name, "test")))
//                .Mutate(x => x.Type, new CarType());


//            var stuff = Modl<IImmutableTable>.Get<TwoValues>(3444);

//            stuff.Modify(new OneValueSave
//            {
//                Value = 2342
//            });




//            var result = change.Commit();

//            var stuff2 = ImmutableModl<IImmutableTable>.Get(234);
//            var stuff3 = ImmutableModl<IImmutableTable>.Get<TwoValues>(234);


//            //ImmutableModl<SimpleValueTable>.Add(new OneValueSave
//            //{
//            //    Value = newVal
//            //});

//            //Assert.True(testClass.IsNew());
//            //Assert.False(testClass.IsModified());

//            //testClass = Modl<EmptyClass>.New();
//            //Assert.True(testClass.IsNew());
//            //Assert.False(testClass.IsModified());

//            //var id = Guid.NewGuid();
//            //testClass = Modl<EmptyClass>.New(id);
//            //Assert.True(testClass.IsNew());
//            //Assert.False(testClass.IsModified());
//            //Assert.True(id == testClass.Id());
//        }
//    }
//}
