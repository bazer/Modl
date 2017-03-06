using ExampleModel;
using Modl;
using Modl.Json;
using Modl.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modl.Exceptions;
using Xunit;

namespace Tests
{
    public class Basics
    {
        public Basics()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CoreStuff()
        {
            Assert.Equal("Id", Modl<ICar>.Definitions.IdProperty.PropertyName);

            //var car = ModlContext.Car.New();

            //var car4 = M.Get<ICar>(456);
            //var mutation = car4
            //    .Mutate(x => x.Name, "KASdA")
            //    .Mutate();

            //var mutCar = car4.Mutate();

            //var mutation2 = mutCar.ToMutation();
            //var commit = mutation2.Commit();
            
            //mutation.Name = "fsdfs";
            //mutation.Tags


            //var car = Modl<ICar>.New();
            var car = M.New<ICar>();
            Assert.True(car.IsMutable);
            Assert.True(car.IsNew);
            Assert.False(car.IsModified);

            Assert.Null(car.Name);
            car.Name = "Mazda";
            Assert.True(car.IsModified);
            Assert.Equal("Mazda", car.Name);
            Assert.Equal("Mazda", car["Name"]);
            car["Name"] = "BMW";
            Assert.Equal("BMW", car.Name);

            Assert.Throws<InvalidPropertyNameException>(() => car["WrongName"]);
            Assert.Throws<InvalidPropertyNameException>(() => car["WrongName"] = "test");

            var mutation = car.ToMutation();
            Assert.Equal(1, mutation.Modifications.Count());
            //Assert.Null(changes.Modifications.First().Modl);
            Assert.Equal("Name", mutation.Modifications.First().NewProperty.Name);
            Assert.Equal("BMW", (mutation.Modifications.First().NewProperty as ISimpleProperty).Value);


            car.Manufacturer = M.New<IManufacturer>()
                .Mutate(x => x.Name, "Ford");

            Assert.NotNull(car.Manufacturer);
            Assert.Equal("Ford", car.Manufacturer.Name);
            Assert.True(car.Manufacturer.IsMutable);
            Assert.True(car.Manufacturer.IsNew);
            Assert.True(car.Manufacturer.IsModified);

            mutation = car.ToMutation();
            Assert.Equal(2, mutation.Modifications.Count());
            Assert.Equal("Manufacturer", mutation.Modifications.Last().NewProperty.Name);
            //Assert.Equal(car.Manufacturer, (changes.Modifications.Last().Property as ISimpleProperty).Value);


           



            //manufacturer
            //    .Mutate(x => x.Name, "Ford")
            //    .AppendTo(changes)
            //    .Commit();



            //var car2 = car.Mutate();
            //car2.Name = "dadsdfsdf";
            //car2.Type = new CarType { Description = "sdfsdg" };
            //car2.Commit();


            //car.Name = "Audi";
            //Assert.True(car.IsNew);
            //Assert.True(car.IsModified);

            //car = Modl<ICar>.New();
            //Assert.True(car.IsNew);
            //Assert.False(car.IsModified);
            //car.Name = "Audi";
            //Assert.True(car.IsNew);
            //Assert.True(car.IsModified);

            //car = new Car().Modl().Modl().Modl().Modl();
            //Assert.True(car.IsNew());
            //Assert.False(car.IsModified());
            //car.Name = "Audi";
            //Assert.True(car.IsNew());
            //Assert.True(car.IsModified());

            //car.Save();

            //car = Modl<ICar>.New();
            ////car.Manufacturer = Modl<IManufacturer>.New().AsMutable() new Manufacturer("BMW");
            //Assert.True(car.IsNew());
            //Assert.True(car.IsModified());
            //Assert.Equal("BMW", car.Manufacturer.Name);
            //Assert.True(car.Manufacturer.IsNew());
            //Assert.True(car.Manufacturer.IsModified());
        }



        //public TimeSpan PerformanceCRUD(string databaseName, int iterations, CacheLevel cache)
        //{
        //    Config.CacheLevel = cache;
        //    //SwitchDatabase(databaseName);

        //    var db = Database.Get(databaseName);
        //    var watch = Stopwatch.StartNew();

        //    for (int i = 0; i < iterations; i++)
        //        CRUD(db);

        //    watch.Stop();
        //    Console.WriteLine(string.Format("{0} iterations for {1}: {2} ms. (cache {3})", iterations, databaseName, watch.Elapsed.TotalMilliseconds, cache));

        //    return watch.Elapsed;
        //}


        [Fact]
        public void CRUD()
        {
            //Modl<Car>.Settings.ConfigurePipeline(new JsonModl<Car>(), new FileModl<Car>());

            //ModlConfig.GlobalSettings.Serializer = new JsonModl();
            //ModlConfig.GlobalSettings.Endpoint = new FileModl();
            //Modl<Manufacturer>.Settings.Serializer = new JsonModl();
            //Modl<Manufacturer>.Settings.Endpoint = new FileModl();

            //Car car = Modl<Car>.New();

            var car = M.New<ICar>();
            Assert.False(car.IsModified);
            car.Name = "M3";
            car.Manufacturer = M.New<IManufacturer>().Mutate(x => x.Name, "BMW");
            car.Type = new CarType();
            car.Type.Description = "Sedan";
            car.Tags = new List<string>();
            car.Tags.Add("Nice");
            car.Tags.Add("Fast");
            car.Tags.Add("Blue");
            car.Tags.Add("Awful");

            Assert.Equal("Sedan", car.Type.Description);
            Assert.True(car.IsModified);

            var mutation = car.ToMutation();
            Assert.Equal(4, mutation.Modifications.Count());
            var user = new User("Basic user");
            var commit = mutation.Commit(user);

            Assert.True(commit.When > DateTime.UtcNow.AddMilliseconds(-100));
            Assert.True(commit.When <= DateTime.UtcNow);
            Assert.Equal(0, commit.Previous.Count());
            Assert.Equal(0, commit.Next.Count());
            Assert.NotEqual(Guid.Empty, commit.Id);
            Assert.Equal(mutation.Modifications.Count(), commit.Modifications.Count());
            Assert.Equal(user, commit.User);

            commit.Push();

            //car.Save();
            Assert.False(car.IsNew);
            Assert.False(car.IsModified);
            car.Manufacturer.Save();

            var car2 = M.Get<ICar>(car.Id);
            AssertEqual(car, car2);
            Assert.Equal("Sedan", car2.Type.Description);
            car2.Manufacturer.Name = "Mercedes";
            Assert.Equal("Mercedes", car2.Manufacturer.Name);
            car2.Manufacturer.Save();

            Car car3 = Modl<Car>.Get(car.Id);
            Assert.Equal("Mercedes", car3.Manufacturer.Val.Name);
            car3.Delete();
            Assert.True(car3.IsDeleted());
            Assert.False(car3.Manufacturer.Val.IsDeleted());
            car3.Manufacturer.Val.Delete();
            Assert.True(car3.Manufacturer.Val.IsDeleted());
            Assert.Throws<NotFoundException>(() => Modl<Car>.Get(car.Id));
        }

        [Fact]
        public void CRUDExplicitId()
        {
            var m1 = M.New<IManufacturer>();
            Assert.False(m1.IsModified());
            m1.Name = "BMW";
            Assert.True(m1.IsModified());
            m1.Save();
            Assert.False(m1.IsNew());
            Assert.False(m1.IsModified());

            var m2 = M.Get<IManufacturer>(m1.ManufacturerID);
            AssertEqual(m1, m2);

            m2.Name = "Mercedes";
            Assert.Equal("Mercedes", m2.Name);
            m2.Save();

            Manufacturer m3 = Modl<Manufacturer>.Get(m1.ManufacturerID);
            Assert.Equal("Mercedes", m3.Name);
            m3.Delete();
            Assert.True(m3.IsDeleted());
            Assert.Throws<NotFoundException>(() => Modl<Manufacturer>.Get(m1.ManufacturerID));
        }

        ////public void CRUDTransaction(Database database = null)
        ////{
        ////    Transaction.Start();

        ////    using (var trans = database.StartTransaction())
        ////    {
        ////        Car car = Car.New(database);
        ////        Assert.Equal(false, car.IsDirty);
        ////        car.Name = "M3";
        ////        car.Manufacturer = "BMW";
        ////        Assert.Equal(true, car.IsDirty);
        ////        car.Save();
        ////        Assert.True(!car.IsNew);
        ////        Assert.Equal(false, car.IsDirty);

        ////        Car car2 = Car.Get(car.Id, database);
        ////        AssertEqual(car, car2);

        ////        car2.Manufacturer = "Mercedes";
        ////        Assert.Equal("Mercedes", car2.Manufacturer);
        ////        car2.Save();

        ////        Car car3 = Car.Get(car.Id, database);
        ////        Assert.Equal("Mercedes", car3.Manufacturer);
        ////        car3.Delete();
        ////        Assert.True(car3.IsDeleted);
        ////        Assert.Equal(null, Car.Get(car.Id, database));

        ////        trans.Commit();
        ////    }

        ////}


      

        //public void SwitchStaticDatabaseAndCRUD(string databaseName)
        //{
        //    DbModl<Car>.DefaultDatabase = Database.Get(databaseName);
        //    Assert.Equal(databaseName, DbModl<Car>.DefaultDatabase.Name);

        //    CRUD();

        //    DbModl<Car>.DefaultDatabase = null;
        //}

        //public void SwitchInstanceDatabaseAndCRUD(string databaseName)
        //{
        //    CRUD(Database.Get(databaseName));
        //}

        //public void GetFromDatabaseProvider(string databaseName)
        //{
        //    var db = Database.Get(databaseName);

        //    var car = DbModl<Car>.New(db); // db.New<Car, int>();
        //    car.Manufacturer = new Manufacturer("Saab");
        //    car.Name = "9000";
        //    car.WriteToDb();

        //    var car2 = DbModl<Car>.Get(car.Id, db); // db.Get<Car, int>(car.Id);
        //    AssertEqual(car, car2);

        //    car2.DeleteFromDb();
        //}

        //public void GetFromLinq()
        //{
        //    var car = new Car();
        //    car.Manufacturer = new Manufacturer("Saab");
        //    car.Name = "9000";
        //    car.WriteToDb();

        //    var cars = DbModl<Car>.Query().Where(x => x.Id == car.Id).ToList();
        //    Assert.Equal(1, cars.Count);

        //    var car2 = cars.First();
        //    AssertEqual(car, car2);

        //    car2.DeleteFromDb();

        //    Car c = new Car();

        //}

        //public void GetFromLinqInstance(string databaseName)
        //{
        //    var db = Database.Get(databaseName);

        //    var car = DbModl<Car>.New(db); //db.New<Car, int>();
        //    car.Manufacturer = new Manufacturer("Saab");
        //    car.Name = "9000";
        //    car.WriteToDb();

        //    var cars = DbModl<Car>.Query(db).Where(x => x.Id == car.Id).ToList();
        //    Assert.Equal(1, cars.Count);

        //    var selectList = DbModl<Car>.Query(db).Where(x => x.Name != "dsklhfsd").AsEnumerable().AsSelectList(x => x.Manufacturer.Name + " " + x.Name);
        //    Assert.True(selectList.Count() > 0);

        //    //var car2 = cars.First();
        //    var car2 = DbModl<Car>.Query(db).Where(x => x.Id == car.Id).First();
        //    AssertEqual(car, car2);

        //    var car3 = DbModl<Car>.GetWhere(x => x.Name == "9000", db);
        //    Assert.Equal("9000", car3.Name);

        //    car2.DeleteFromDb();
        //}

        //public void GetFromLinqAdvanced(string databaseName)
        //{
        //    var db = Database.Get(databaseName);

        //    var car = DbModl<Car>.New(db);
        //    car.Manufacturer = new Manufacturer("Saab");
        //    car.Name = "9000";
        //    car.WriteToDb();

        //    var cars = DbModl<Car>.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").ToList();
        //    Assert.Equal(1, cars.Count);

        //    var car2 = DbModl<Car>.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").First();
        //    AssertEqual(car, car2);

        //    car2.DeleteFromDb();
        //}

        //public void StaticDelete()
        //{
        //    var cars = NewCars(5);
        //    Assert.True(DbModl<Car>.GetAll().Count() >= cars.Count);

        //    DbModl<Car>.DeleteAll();
        //    Assert.Equal(0, DbModl<Car>.GetAll().Count());

        //    cars = NewCars(5);
        //    Assert.Equal(5, DbModl<Car>.GetAll().Count());

        //    DbModl<Car>.Delete(cars[0].Id);
        //    Assert.False(DbModl<Car>.Exists(cars[0].Id));
        //    Assert.Equal(4, DbModl<Car>.GetAll().Count());

        //    cars[1].Name = "10000";
        //    cars[1].WriteToDb();
        //    DbModl<Car>.DeleteAllWhere(x => x.Name == "9000");
        //    Assert.True(DbModl<Car>.Exists(cars[1].Id));
        //    Assert.Equal(1, DbModl<Car>.GetAll().Count());

        //    cars[1].DeleteFromDb();
        //    Assert.Equal(0, DbModl<Car>.GetAll().Count());
        //}

        //public List<Car> NewCars(int count, bool save = true)
        //{
        //    List<Car> list = new List<Car>(count);

        //    while (count-- > 0)
        //    {
        //        var car = new Car();
        //        car.Manufacturer = new Manufacturer("Saab");
        //        car.Name = "9000";

        //        if (save)
        //            car.WriteToDb();

        //        list.Add(car);
        //    }

        //    return list;
        //}

        public void AssertEqual(ICar car1, ICar car2)
        {
            //Assert.Equal(car1.Database(), car2.Database());
            //Assert.Equal(car1.Database().Name, car2.Database().Name);
            Assert.Equal(car1.Id, car2.Id);
            Assert.Equal(car1.Tags.Count, car2.Tags.Count);
            Assert.Equal(car1.Tags[0], car2.Tags[0]);
            Assert.Equal(car1.Type.Description, car2.Type.Description);
            AssertEqual(car1.Manufacturer, car2.Manufacturer);
            Assert.Equal(car1.Name, car2.Name);
        }

        public void AssertEqual(IManufacturer m1, IManufacturer m2)
        {
            //Assert.Equal(m1.Database(), m2.Database());
            //Assert.Equal(m1.Database().Name, m2.Database().Name);
            Assert.Equal(m1.ManufacturerID, m2.ManufacturerID);
            Assert.Equal(m1.Name, m2.Name);
        }

        [Fact]
        public void SetIdExplicit()
        {
            var id = Guid.NewGuid();
            var m1 = M.New<IManufacturer>().Mutate(x => x.ManufacturerID, id);
            m1.Name = "Audi";
            Assert.Equal(id, m1.ManufacturerID);
            m1.Save();
            Assert.Equal(id, m1.ManufacturerID);

            var m2 = M.Get<IManufacturer>(m1.ManufacturerID.ToString());
            AssertEqual(m1, m2);

            m2.Save();
            Assert.Equal(id, m2.ManufacturerID);

            m2.Delete();
        }

        //public void GetAllAsync()
        //{
        //    //DbModl<Car>.DeleteAll();

        //    //NewCars(10);
        //    ////Thread.Sleep(100);

        //    //Assert.Equal(10, DbModl<Car>.GetAll().Count());

        //    //List<Task<Car>> carsAsync = new List<Task<Car>>();
        //    //var cars = DbModl<Car>.GetAll().ToList();
        //    //foreach (var car in cars)
        //    //    carsAsync.Add(Car.GetAsync(car.Id));

        //    //for (int i = 0; i < cars.Count; i++)
        //    //{
        //    //    AssertEqual(cars[i], carsAsync[i].Result);
        //    //    carsAsync[i].Result.DeleteFromDb();
        //    //}

        //    //Assert.Equal(0, DbModl<Car>.GetAll().Count());
        //}
    }
}
