/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using ExampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.Mvc;
using Modl.DatabaseProviders;
using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

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

        

        public void SwitchDatabase(string databaseName)
        {
            Car.DefaultDatabase = null;
            Manufacturer.DefaultDatabase = null;
            Database.Default = Database.Get(databaseName);

            Assert.AreEqual(databaseName, Database.Default.Name);
            Assert.AreEqual(Database.Default, Car.DefaultDatabase);
            Assert.AreEqual(Database.Default, Car.New().Database);
        }

        public void CRUD(Database database = null)
        {
            Car car = NewModl<Car, int>(database);
            Assert.AreEqual(false, car.IsDirty);
            car.Name = "M3";
            car.Manufacturer = "BMW";
            Assert.AreEqual(true, car.IsDirty);
            car.Save();
            Assert.IsTrue(!car.IsNew);
            Assert.AreEqual(false, car.IsDirty);

            Car car2 = GetModl<Car, int>(car.Id, database); // Car.Get(car.Id);
            AssertEqual(car, car2);

            car2.Manufacturer = "Mercedes";
            Assert.AreEqual("Mercedes", car2.Manufacturer);
            car2.Save();

            Car car3 = GetModl<Car, int>(car.Id, database);
            Assert.AreEqual("Mercedes", car3.Manufacturer);
            car3.Delete();
            Assert.IsTrue(car3.IsDeleted);
            Assert.AreEqual(null, GetModl<Car, int>(car.Id, database));

            
        }

        public void CRUDExplicitId(Database database)
        {
            Manufacturer m1 = Manufacturer.New(Guid.NewGuid(), database);
            Assert.AreEqual(false, m1.IsDirty);
            m1.Name = "BMW";
            Assert.AreEqual(true, m1.IsDirty);
            m1.Save();
            Assert.IsTrue(!m1.IsNew);
            Assert.AreEqual(false, m1.IsDirty);

            Manufacturer m2 = Manufacturer.Get(m1.Id, database);
            AssertEqual(m1, m2);

            m2.Name = "Mercedes";
            Assert.AreEqual("Mercedes", m2.Name);
            m2.Save();

            Manufacturer m3 = Manufacturer.Get(m1.Id, database);
            Assert.AreEqual("Mercedes", m3.Name);
            m3.Delete();
            Assert.IsTrue(m3.IsDeleted);
            Assert.AreEqual(null, Manufacturer.Get(m1.Id, database));
        }



        public T NewModl<T, IdType>(Database database) where T : Modl<T, IdType>, new()
        {
            T modl;

            if (database == null)
                modl = Modl<T, IdType>.New();
            else
                modl = Modl<T, IdType>.New(database);

            Assert.IsTrue(modl.IsNew);

            return modl;
        }

        public T GetModl<T, IdType>(IdType id, Database database) where T : Modl<T, IdType>, new()
        {
            T modl = Modl<T, IdType>.Get(id, database);

            if (modl != null)
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

            var car = Car.New(db); // db.New<Car, int>();
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();

            var car2 = Car.Get(car.Id, db); // db.Get<Car, int>(car.Id);
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

            var car = Car.New(db); //db.New<Car, int>();
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();
            
            var cars = Car.Query(db).Where(x => x.Id == car.Id).ToList();
            Assert.AreEqual(1, cars.Count);

            var selectList = Car.Query(db).Where(x => x.Name != "dsklhfsd").AsEnumerable().AsSelectList<Car, int>(x => x.Manufacturer + " " + x.Name);
            Assert.IsTrue(selectList.Count() > 0);
            
            //var car2 = cars.First();
            var car2 = Car.Query(db).Where(x => x.Id == car.Id).First();
            AssertEqual(car, car2);
            
            var car3 = Car.GetWhere(x => x.Name == "9000", db);
            Assert.AreEqual("9000", car3.Name);

            car2.Delete();
        }

        public void GetFromLinqAdvanced(string databaseName)
        {
            var db = Database.Get(databaseName);

            var car = Car.New(db);
            car.Manufacturer = "Saab";
            car.Name = "9000";
            car.Save();

            var cars = Car.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").ToList();
            Assert.AreEqual(1, cars.Count);

            var car2 = Car.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").First();
            AssertEqual(car, car2);
            
            car2.Delete();
        }

        public void StaticDelete()
        {
            var cars = NewCars(5);
            Assert.IsTrue(Car.GetAll().Count() >= cars.Count);

            Car.DeleteAll();
            Assert.AreEqual(0, Car.GetAll().Count());

            cars = NewCars(5);
            Assert.AreEqual(5, Car.GetAll().Count());

            Car.Delete(cars[0].Id);
            Assert.IsFalse(Car.Exists(cars[0].Id));
            Assert.AreEqual(4, Car.GetAll().Count());

            cars[1].Name = "10000";
            cars[1].Save();
            Car.DeleteAllWhere(x => x.Name == "9000");
            Assert.IsTrue(Car.Exists(cars[1].Id));
            Assert.AreEqual(1, Car.GetAll().Count());

            cars[1].Delete();
            Assert.AreEqual(0, Car.GetAll().Count());
        }

        public List<Car> NewCars(int count, bool save = true)
        {
            List<Car> list = new List<Car>(count);

            while (count-- > 0)
            {
                var car = new Car();
                car.Manufacturer = "Saab";
                car.Name = "9000";

                if (save)
                    car.Save();

                list.Add(car);
            }

            return list;
        }

        public void AssertEqual(Car car1, Car car2)
        {
            Assert.AreEqual(car1.Database, car2.Database);
            Assert.AreEqual(car1.Database.Name, car2.Database.Name);
            Assert.AreEqual(car1.Id, car2.Id);
            Assert.AreEqual(car1.Manufacturer, car2.Manufacturer);
            Assert.AreEqual(car1.Name, car2.Name);
        }

        public void AssertEqual(Manufacturer m1, Manufacturer m2)
        {
            Assert.AreEqual(m1.Database, m2.Database);
            Assert.AreEqual(m1.Database.Name, m2.Database.Name);
            Assert.AreEqual(m1.Id, m2.Id);
            Assert.AreEqual(m1.Name, m2.Name);
        }

        public void SetIdExplicit()
        {
            var id = Guid.NewGuid();
            Manufacturer m1 = Manufacturer.New(id);
            m1.Name = "Audi";
            Assert.AreEqual(id, m1.Id);
            m1.Save();
            Assert.AreEqual(id, m1.Id);

            var m2 = Manufacturer.Get(m1.Id);
            AssertEqual(m1, m2);

            m2.Save();
            Assert.AreEqual(id, m2.Id);

            m2.Delete();
        }

        public void GetAllAsync()
        {
            Car.DeleteAll();

            NewCars(10);
            //Thread.Sleep(100);

            Assert.AreEqual(10, Car.GetAll().Count());

            List<Task<Car>> carsAsync = new List<Task<Car>>();
            var cars = Car.GetAll().ToList();
            foreach (var car in cars)
                carsAsync.Add(Car.GetAsync(car.Id));

            for (int i = 0; i < cars.Count; i++)
            {
                AssertEqual(cars[i], carsAsync[i].Result);
                carsAsync[i].Result.Delete();
            }

            Assert.AreEqual(0, Car.GetAll().Count());
        }
    }
}
