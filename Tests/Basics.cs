using ExampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Mvc;
using Modl.DatabaseProviders;
using System.Linq;
using System.Diagnostics;
using System;

namespace Tests
{
    [TestClass]
    public class Basics
    {
        [TestMethod]
        public void CRUDDatabases()
        {
            var databases = Database.GetAll();
            Assert.IsTrue(databases.Count > 0);

            Database.Remove("SqlServerDb");
            Assert.AreEqual(databases.Count - 1, Database.GetAll().Count);

            Database.RemoveAll();
            Assert.AreEqual(0, Database.GetAll().Count);

            foreach (var db in databases)
                Database.Add(db);

            Assert.AreEqual(databases.Count, Database.GetAll().Count);
        }

        public TimeSpan PerformanceCRUD(string databaseName, int iterations)
        {
            SwitchDatabase(databaseName);

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                CRUD();

            watch.Stop();
            Console.WriteLine(string.Format("{0} iterations for {1}: {2} ms.", iterations, databaseName, watch.Elapsed.TotalMilliseconds));

            return watch.Elapsed;
        }


        public void SwitchDatabase(string databaseName)
        {
            Database.Default = Database.Get(databaseName);

            Assert.AreEqual(databaseName, Database.Default.Name);
            Assert.AreEqual(Database.Default, Car.DefaultDatabase);
            Assert.AreEqual(Database.Default, Car.New().Database);
        }

        public void CRUD(Database database = null)
        {
            Car car = NewModl<Car>(database);
            Assert.AreEqual(false, car.IsDirty);
            car.Name = "M3";
            car.Manufacturer = "BMW";
            Assert.AreEqual(true, car.IsDirty);
            car.Save();
            Assert.IsTrue(!car.IsNew);
            Assert.AreEqual(false, car.IsDirty);

            Car car2 = GetModl<Car>(car.Id, database); // Car.Get(car.Id);
            AssertEqual(car, car2);

            car2.Manufacturer = "Mercedes";
            Assert.AreEqual("Mercedes", car2.Manufacturer);
            car2.Save();

            Car car3 = GetModl<Car>(car.Id, database);
            Assert.AreEqual("Mercedes", car3.Manufacturer);
            car3.Delete();
            Assert.IsTrue(car3.IsDeleted);
            Assert.AreEqual(null, GetModl<Car>(car.Id, database, false));

            
        }

        public T NewModl<T>(Database database) where T : Modl<T>, new()
        {
            T modl;

            if (database == null)
                modl = Modl<T>.New();
            else
                modl = Modl<T>.New(database);

            Assert.IsTrue(modl.IsNew);

            return modl;
        }

        public T GetModl<T>(int id, Database database, bool throwExceptionOnNotFound = true) where T : Modl<T>, new()
        {
            T modl = Modl<T>.Get(id, database, throwExceptionOnNotFound);

            if (!throwExceptionOnNotFound && modl != null)
                Assert.IsTrue(!modl.IsNew);

            return modl;
        }

        public void SwitchStaticDatabaseAndCRUD(string databaseName)
        {
            Car.DefaultDatabase = Database.Get(databaseName);
            Assert.AreEqual(databaseName, Car.DefaultDatabase.Name);

            CRUD();

            Car.DefaultDatabase = null;
        }

        public void SwitchInstanceDatabaseAndCRUD(string databaseName)
        {
            CRUD(Database.Get(databaseName));
        }

        public void GetFromDatabaseProvider(string databaseName)
        {
            var db = Database.Get(databaseName);

            var car = db.New<Car>();
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();

            var car2 = db.Get<Car>(car.Id);
            AssertEqual(car, car2);

            car2.Delete();
        }

        public void GetFromLinq()
        {
            var car = new Car();
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();

            var cars = Car.Query().Where(x => x.Id == car.Id).ToList();
            Assert.AreEqual(1, cars.Count);

            var car2 = cars.First();
            AssertEqual(car, car2);
            
            car2.Delete();

            Car c = new Car();
            
        }

        public void GetFromLinqInstance(string databaseName)
        {
            var db = Database.Get(databaseName);
            
            var car = db.New<Car>();
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();
            
            var cars = db.Query<Car>().Where(x => x.Id == car.Id).ToList();
            Assert.AreEqual(1, cars.Count);

            var selectList = db.Query<Car>().Where(x => x.Name != "dsklhfsd").AsEnumerable().AsSelectList(x => x.Manufacturer + " " + x.Name);
            Assert.IsTrue(selectList.Count() > 0);
            
            var car2 = cars.First();
            AssertEqual(car, car2);

            //var car2 = Car.Query(db).Where(x => x.Id == car.Id).First();
            //var car3 = Car.GetWhere(x => x.Name == "9000", db);
            //Assert.AreEqual("9000", car3.Name);

            car2.Delete();
        }

        public void GetFromLinqAdvanced(string databaseName)
        {
            var db = Database.Get(databaseName);

            var car = db.New<Car>();
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();

            var cars = db.Query<Car>().Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").ToList();
            Assert.AreEqual(1, cars.Count);

            var car2 = Car.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").First();
            AssertEqual(car, car2);
            
            car2.Delete();
        }

        public void AssertEqual(Car car1, Car car2)
        {
            Assert.AreEqual(car1.Database, car2.Database);
            Assert.AreEqual(car1.Database.Name, car2.Database.Name);
            Assert.AreEqual(car1.Id, car2.Id);
            Assert.AreEqual(car1.Manufacturer, car2.Manufacturer);
            Assert.AreEqual(car1.Name, car2.Name);
        }
    }
}
