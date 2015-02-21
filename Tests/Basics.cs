﻿/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

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
using Modl.Db.DatabaseProviders;
using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Modl.Db;
using Modl.Json;
using Modl.Plugins;

namespace Tests
{
    [TestClass]
    public class Basics
    {
        [TestMethod]
        public void CoreStuff()
        {
            


            Assert.AreEqual("Vehicle_fk", Modl<Car>.Metadata.IdName);

            var car = Modl<Car>.New();
            Assert.IsTrue(car.IsNew());
            Assert.IsFalse(car.IsModified());
            car.Name = "Audi";
            Assert.IsTrue(car.IsNew());
            Assert.IsTrue(car.IsModified());

            car = new Car().Modl();
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
            car.Manufacturer = new Manufacturer("BMW");
            Assert.IsTrue(car.IsNew());
            Assert.IsTrue(car.IsModified());
            Assert.AreEqual("BMW", car.Manufacturer.Name);
            Assert.IsTrue(car.Manufacturer.IsNew());
            Assert.IsTrue(car.Manufacturer.IsModified());
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
            
            ModlConfig.GlobalSettings.Serializer = new JsonModl();
            ModlConfig.GlobalSettings.Endpoint = new FileModl();
            Modl<Manufacturer>.Settings.Serializer = new JsonModl();
            Modl<Manufacturer>.Settings.Endpoint = new FileModl();

            Car car = Modl<Car>.New();

            Assert.AreEqual(false, car.IsModified());
            car.Name = "M3";
            car.Manufacturer = new Manufacturer("BMW");
            Assert.AreEqual(true, car.IsModified());
            car.Save();
            Assert.IsTrue(!car.IsNew());
            Assert.AreEqual(false, car.IsModified());

            Car car2 = GetModl<Car>(car.Id); // Car.Get(car.Id);
            AssertEqual(car, car2);

            car2.Manufacturer.Name = "Mercedes";
            Assert.AreEqual("Mercedes", car2.Manufacturer.Name);
            car2.Manufacturer.Save();

            Car car3 = GetModl<Car>(car.Id);
            Assert.AreEqual("Mercedes", car3.Manufacturer.Name);
            //car3.DeleteFromDb();
            Assert.IsTrue(car3.IsDeleted());
            Assert.AreEqual(null, GetModl<Car>(car.Id));

        }

        //public void CRUDExplicitId(Database database)
        //{
        //    Manufacturer m1 = DbModl<Manufacturer>.New(Guid.NewGuid(), database);
        //    Assert.AreEqual(true, m1.IsDirty());
        //    m1.Name = "BMW";
        //    Assert.AreEqual(true, m1.IsDirty());
        //    m1.WriteToDb();
        //    Assert.IsTrue(!m1.IsNew());
        //    Assert.AreEqual(false, m1.IsDirty());

        //    Manufacturer m2 = DbModl<Manufacturer>.Get(m1.ManufacturerID, database);
        //    AssertEqual(m1, m2);

        //    m2.Name = "Mercedes";
        //    Assert.AreEqual("Mercedes", m2.Name);
        //    m2.WriteToDb();

        //    Manufacturer m3 = DbModl<Manufacturer>.Get(m1.GetId(), database);
        //    Assert.AreEqual("Mercedes", m3.Name);
        //    m3.DeleteFromDb();
        //    Assert.IsTrue(m3.IsDeleted());
        //    Assert.AreEqual(null, DbModl<Manufacturer>.Get(m1.ManufacturerID, database));
        //}

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
                Assert.IsTrue(!modl.IsNew());

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
            AssertEqual(car1.Manufacturer, car2.Manufacturer);
            //Assert.AreEqual(car1.Manufacturer, car2.Manufacturer);
            Assert.AreEqual(car1.Name, car2.Name);
        }

        public void AssertEqual(Manufacturer m1, Manufacturer m2)
        {
            //Assert.AreEqual(m1.Database(), m2.Database());
            //Assert.AreEqual(m1.Database().Name, m2.Database().Name);
            Assert.AreEqual(m1.GetId(), m2.GetId());
            Assert.AreEqual(m1.Name, m2.Name);
        }

        //public void SetIdExplicit()
        //{
        //    var id = Guid.NewGuid();
        //    Manufacturer m1 = DbModl<Manufacturer>.New(id);
        //    m1.Name = "Audi";
        //    Assert.AreEqual(id, m1.GetId());
        //    m1.WriteToDb();
        //    Assert.AreEqual(id, m1.GetId());

        //    var m2 = DbModl<Manufacturer>.Get(m1.GetId());
        //    AssertEqual(m1, m2);

        //    m2.WriteToDb();
        //    Assert.AreEqual(id, m2.GetId());

        //    m2.DeleteFromDb();
        //}

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
