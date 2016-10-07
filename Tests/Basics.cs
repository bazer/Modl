using ExampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Json;
using Modl.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class Basics
    {
        [TestInitialize]
        public void Initialize()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl();
        }

        [TestMethod]
        public void CoreStuff()
        {
            Assert.AreEqual("Id", Modl<Car>.Definitions.IdProperty.PropertyName);

            var car = Modl<Car>.New();
            Assert.IsTrue(car.IsNew());
            Assert.IsFalse(car.IsModified());
            car.Name = "Audi";
            Assert.IsTrue(car.IsNew());
            Assert.IsTrue(car.IsModified());

            car = new Car();
            Assert.IsTrue(car.IsNew());
            Assert.IsFalse(car.IsModified());
            car.Name = "Audi";
            Assert.IsTrue(car.IsNew());
            Assert.IsTrue(car.IsModified());

            car = new Car().Modl().Modl().Modl().Modl();
            Assert.IsTrue(car.IsNew());
            Assert.IsFalse(car.IsModified());
            car.Name = "Audi";
            Assert.IsTrue(car.IsNew());
            Assert.IsTrue(car.IsModified());

            //car.Save();

            car = Modl<Car>.New();
            car.Manufacturer.Val = new Manufacturer("BMW");
            Assert.IsTrue(car.IsNew());
            Assert.IsTrue(car.IsModified());
            Assert.AreEqual("BMW", car.Manufacturer.Val.Name);
            Assert.IsTrue(car.Manufacturer.Val.IsNew());
            Assert.IsTrue(car.Manufacturer.Val.IsModified());
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


        [TestMethod]
        public void CRUD()
        {
            //Modl<Car>.Settings.ConfigurePipeline(new JsonModl<Car>(), new FileModl<Car>());

            //ModlConfig.GlobalSettings.Serializer = new JsonModl();
            //ModlConfig.GlobalSettings.Endpoint = new FileModl();
            //Modl<Manufacturer>.Settings.Serializer = new JsonModl();
            //Modl<Manufacturer>.Settings.Endpoint = new FileModl();

            //Car car = Modl<Car>.New();

            var car = new Car();
            Assert.IsFalse(car.IsModified());
            car.Name = "M3";
            car.Manufacturer.Val = new Manufacturer("BMW");
            car.Type = new CarType();
            car.Type.Description = "Sedan";
            car.Tags = new List<string>();
            car.Tags.Add("Nice");
            car.Tags.Add("Fast");
            car.Tags.Add("Blue");
            car.Tags.Add("Awful");

            Assert.AreEqual("Sedan", car.Type.Description);
            Assert.IsTrue(car.IsModified());
            car.Save();
            Assert.IsFalse(car.IsNew());
            Assert.IsFalse(car.IsModified());

            Car car2 = GetModl<Car>(car.Id);
            AssertEqual(car, car2);
            Assert.AreEqual("Sedan", car2.Type.Description);
            car2.Manufacturer.Val.Name = "Mercedes";
            Assert.AreEqual("Mercedes", car2.Manufacturer.Val.Name);
            car2.Manufacturer.Val.Save();

            Car car3 = GetModl<Car>(car.Id);
            Assert.AreEqual("Mercedes", car3.Manufacturer.Val.Name);
            car3.Delete();
            Assert.IsTrue(car3.IsDeleted());
            Assert.IsFalse(car3.Manufacturer.Val.IsDeleted());
            car3.Manufacturer.Val.Delete();
            Assert.IsTrue(car3.Manufacturer.Val.IsDeleted());

            try
            {
                Modl<Car>.Get(car.Id);
                Assert.Fail();
            }
            catch (FileNotFoundException) { }
            //catch (Exception)
            //{
            //    Assert.Fail();
            //}
        }

        [TestMethod]
        public void CRUDExplicitId()
        {
            Manufacturer m1 = Modl<Manufacturer>.New(Guid.NewGuid());
            Assert.IsFalse(m1.IsModified());
            m1.Name = "BMW";
            Assert.IsTrue(m1.IsModified());
            m1.Save();
            Assert.IsFalse(m1.IsNew());
            Assert.IsFalse(m1.IsModified());

            Manufacturer m2 = Modl<Manufacturer>.Get(m1.ManufacturerID);
            AssertEqual(m1, m2);

            m2.Name = "Mercedes";
            Assert.AreEqual("Mercedes", m2.Name);
            m2.Save();

            Manufacturer m3 = Modl<Manufacturer>.Get(m1.ManufacturerID);
            Assert.AreEqual("Mercedes", m3.Name);
            m3.Delete();
            Assert.IsTrue(m3.IsDeleted());

            try
            {
                var m4 = Modl<Manufacturer>.Get(m1.ManufacturerID);
                Assert.Fail();
            }
            catch (FileNotFoundException) { }
            //catch (Exception)
            //{
            //    Assert.Fail();
            //}
        }

        ////public void CRUDTransaction(Database database = null)
        ////{
        ////    Transaction.Start();

        ////    using (var trans = database.StartTransaction())
        ////    {
        ////        Car car = Car.New(database);
        ////        Assert.AreEqual(false, car.IsDirty);
        ////        car.Name = "M3";
        ////        car.Manufacturer = "BMW";
        ////        Assert.AreEqual(true, car.IsDirty);
        ////        car.Save();
        ////        Assert.IsTrue(!car.IsNew);
        ////        Assert.AreEqual(false, car.IsDirty);

        ////        Car car2 = Car.Get(car.Id, database);
        ////        AssertEqual(car, car2);

        ////        car2.Manufacturer = "Mercedes";
        ////        Assert.AreEqual("Mercedes", car2.Manufacturer);
        ////        car2.Save();

        ////        Car car3 = Car.Get(car.Id, database);
        ////        Assert.AreEqual("Mercedes", car3.Manufacturer);
        ////        car3.Delete();
        ////        Assert.IsTrue(car3.IsDeleted);
        ////        Assert.AreEqual(null, Car.Get(car.Id, database));

        ////        trans.Commit();
        ////    }

        ////}


        public T NewModl<T>() where T : IModl, new()
        {
            var modl = Modl<T>.New();
            Assert.IsTrue(modl.IsNew());

            return modl;
        }

        public T GetModl<T>(object id) where T : IModl, new()
        {
            T modl = Modl<T>.Get(id);

            if (modl != null)
                Assert.IsFalse(modl.IsNew());

            return modl;
        }

        //public void SwitchStaticDatabaseAndCRUD(string databaseName)
        //{
        //    DbModl<Car>.DefaultDatabase = Database.Get(databaseName);
        //    Assert.AreEqual(databaseName, DbModl<Car>.DefaultDatabase.Name);

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
        //    Assert.AreEqual(1, cars.Count);

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
        //    Assert.AreEqual(1, cars.Count);

        //    var selectList = DbModl<Car>.Query(db).Where(x => x.Name != "dsklhfsd").AsEnumerable().AsSelectList(x => x.Manufacturer.Name + " " + x.Name);
        //    Assert.IsTrue(selectList.Count() > 0);

        //    //var car2 = cars.First();
        //    var car2 = DbModl<Car>.Query(db).Where(x => x.Id == car.Id).First();
        //    AssertEqual(car, car2);

        //    var car3 = DbModl<Car>.GetWhere(x => x.Name == "9000", db);
        //    Assert.AreEqual("9000", car3.Name);

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
        //    Assert.AreEqual(1, cars.Count);

        //    var car2 = DbModl<Car>.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").First();
        //    AssertEqual(car, car2);

        //    car2.DeleteFromDb();
        //}

        //public void StaticDelete()
        //{
        //    var cars = NewCars(5);
        //    Assert.IsTrue(DbModl<Car>.GetAll().Count() >= cars.Count);

        //    DbModl<Car>.DeleteAll();
        //    Assert.AreEqual(0, DbModl<Car>.GetAll().Count());

        //    cars = NewCars(5);
        //    Assert.AreEqual(5, DbModl<Car>.GetAll().Count());

        //    DbModl<Car>.Delete(cars[0].Id);
        //    Assert.IsFalse(DbModl<Car>.Exists(cars[0].Id));
        //    Assert.AreEqual(4, DbModl<Car>.GetAll().Count());

        //    cars[1].Name = "10000";
        //    cars[1].WriteToDb();
        //    DbModl<Car>.DeleteAllWhere(x => x.Name == "9000");
        //    Assert.IsTrue(DbModl<Car>.Exists(cars[1].Id));
        //    Assert.AreEqual(1, DbModl<Car>.GetAll().Count());

        //    cars[1].DeleteFromDb();
        //    Assert.AreEqual(0, DbModl<Car>.GetAll().Count());
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

        public void AssertEqual(Car car1, Car car2)
        {
            //Assert.AreEqual(car1.Database(), car2.Database());
            //Assert.AreEqual(car1.Database().Name, car2.Database().Name);
            Assert.AreEqual(car1.Id, car2.Id);
            Assert.AreEqual(car1.Tags.Count, car2.Tags.Count);
            Assert.AreEqual(car1.Tags[0], car2.Tags[0]);
            Assert.AreEqual(car1.Type.Description, car2.Type.Description);
            AssertEqual(car1.Manufacturer.First(), car2.Manufacturer.First());
            Assert.AreEqual(car1.Name, car2.Name);
        }

        public void AssertEqual(Manufacturer m1, Manufacturer m2)
        {
            //Assert.AreEqual(m1.Database(), m2.Database());
            //Assert.AreEqual(m1.Database().Name, m2.Database().Name);
            Assert.AreEqual(m1.ManufacturerID, m2.ManufacturerID);
            Assert.AreEqual(m1.Name, m2.Name);
        }

        [TestMethod]
        public void SetIdExplicit()
        {
            var id = Guid.NewGuid();
            Manufacturer m1 = Modl<Manufacturer>.New(id);
            m1.Name = "Audi";
            Assert.AreEqual(id, m1.ManufacturerID);
            m1.Save();
            Assert.AreEqual(id, m1.ManufacturerID);

            var m2 = Modl<Manufacturer>.Get(m1.ManufacturerID.ToString());
            AssertEqual(m1, m2);

            m2.Save();
            Assert.AreEqual(id, m2.ManufacturerID);

            m2.Delete();
        }

        //public void GetAllAsync()
        //{
        //    //DbModl<Car>.DeleteAll();

        //    //NewCars(10);
        //    ////Thread.Sleep(100);

        //    //Assert.AreEqual(10, DbModl<Car>.GetAll().Count());

        //    //List<Task<Car>> carsAsync = new List<Task<Car>>();
        //    //var cars = DbModl<Car>.GetAll().ToList();
        //    //foreach (var car in cars)
        //    //    carsAsync.Add(Car.GetAsync(car.Id));

        //    //for (int i = 0; i < cars.Count; i++)
        //    //{
        //    //    AssertEqual(cars[i], carsAsync[i].Result);
        //    //    carsAsync[i].Result.DeleteFromDb();
        //    //}

        //    //Assert.AreEqual(0, DbModl<Car>.GetAll().Count());
        //}
    }
}
