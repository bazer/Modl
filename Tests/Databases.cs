//using Xunit;
//using System.Linq;

//namespace Tests
//{
//    
//    public class Databases
//    {
//        [Fact]
//        public void CRUDDatabases()
//        {
//            var databases = Database.GetAll();
//            Assert.True(databases.Count > 0);

//            Database.Remove("SqlServerDb");
//            Assert.Equal(databases.Count - 1, Database.GetAll().Count);

//            Database.RemoveAll();
//            Assert.Equal(0, Database.GetAll().Count);

//            foreach (var db in databases)
//                Database.Add(db);

//            Assert.Equal(databases.Count, Database.GetAll().Count);
//        }

//        //public TimeSpan PerformanceCRUD(string databaseName, int iterations, CacheLevel cache)
//        //{
//        //    Config.CacheLevel = cache;
//        //    //SwitchDatabase(databaseName);

//        //    var db = Database.Get(databaseName);
//        //    var watch = Stopwatch.StartNew();

//        //    for (int i = 0; i < iterations; i++)
//        //        CRUD(db);

//        //    watch.Stop();
//        //    Console.WriteLine(string.Format("{0} iterations for {1}: {2} ms. (cache {3})", iterations, databaseName, watch.Elapsed.TotalMilliseconds, cache));

//        //    return watch.Elapsed;
//        //}

        

//        //public void SwitchDatabase(string databaseName)
//        //{
//        //    Modl<Car>.DefaultDatabase = null;
//        //    Modl<Manufacturer>.DefaultDatabase = null;
//        //    Database.Default = Database.Get(databaseName);

//        //    Assert.Equal(databaseName, Database.Default.Name);
//        //    Assert.Equal(Database.Default, Modl<Car>.DefaultDatabase);
//        //    Assert.Equal(Database.Default, Modl<Car>.New().Database());
//        //}

//        //public void CRUD(Database database = null)
//        //{
//        //    Car car = Modl<Car>.New(database);
            
//        //    Assert.Equal(false, car.IsDirty());
//        //    car.Name = "M3";
//        //    car.Manufacturer = new Manufacturer("BMW");
//        //    Assert.Equal(true, car.IsDirty());
//        //    car.WriteToDb();
//        //    Assert.True(!car.IsNew());
//        //    Assert.Equal(false, car.IsDirty());

//        //    Car car2 = GetModl<Car>(car.Id, database); // Car.Get(car.Id);
//        //    AssertEqual(car, car2);

//        //    car2.Manufacturer.Name = "Mercedes";
//        //    Assert.Equal("Mercedes", car2.Manufacturer.Name);
//        //    car2.Manufacturer.WriteToDb();

//        //    Car car3 = GetModl<Car>(car.Id, database);
//        //    Assert.Equal("Mercedes", car3.Manufacturer.Name);
//        //    car3.DeleteFromDb();
//        //    Assert.True(car3.IsDeleted());
//        //    Assert.Equal(null, GetModl<Car>(car.Id, database));

//        //}

//        //public void CRUDExplicitId(Database database)
//        //{
//        //    Manufacturer m1 = DbModl<Manufacturer>.New(Guid.NewGuid(), database);
//        //    Assert.Equal(true, m1.IsDirty());
//        //    m1.Name = "BMW";
//        //    Assert.Equal(true, m1.IsDirty());
//        //    m1.WriteToDb();
//        //    Assert.True(!m1.IsNew());
//        //    Assert.Equal(false, m1.IsDirty());

//        //    Manufacturer m2 = DbModl<Manufacturer>.Get(m1.ManufacturerID, database);
//        //    AssertEqual(m1, m2);

//        //    m2.Name = "Mercedes";
//        //    Assert.Equal("Mercedes", m2.Name);
//        //    m2.WriteToDb();

//        //    Manufacturer m3 = DbModl<Manufacturer>.Get(m1.GetId(), database);
//        //    Assert.Equal("Mercedes", m3.Name);
//        //    m3.DeleteFromDb();
//        //    Assert.True(m3.IsDeleted());
//        //    Assert.Equal(null, DbModl<Manufacturer>.Get(m1.ManufacturerID, database));
//        //}

//        ////public void CRUDTransaction(Database database = null)
//        ////{
//        ////    Transaction.Start();

//        ////    using (var trans = database.StartTransaction())
//        ////    {
//        ////        Car car = Car.New(database);
//        ////        Assert.Equal(false, car.IsDirty);
//        ////        car.Name = "M3";
//        ////        car.Manufacturer = "BMW";
//        ////        Assert.Equal(true, car.IsDirty);
//        ////        car.Save();
//        ////        Assert.True(!car.IsNew);
//        ////        Assert.Equal(false, car.IsDirty);

//        ////        Car car2 = Car.Get(car.Id, database);
//        ////        AssertEqual(car, car2);

//        ////        car2.Manufacturer = "Mercedes";
//        ////        Assert.Equal("Mercedes", car2.Manufacturer);
//        ////        car2.Save();

//        ////        Car car3 = Car.Get(car.Id, database);
//        ////        Assert.Equal("Mercedes", car3.Manufacturer);
//        ////        car3.Delete();
//        ////        Assert.True(car3.IsDeleted);
//        ////        Assert.Equal(null, Car.Get(car.Id, database));

//        ////        trans.Commit();
//        ////    }

//        ////}


//        //public T NewModl<T>(Database database) where T : IDbModl, new()
//        //{
//        //    T modl;

//        //    if (database == null)
//        //        modl = DbModl<T>.New();
//        //    else
//        //        modl = DbModl<T>.New(database);

//        //    Assert.True(modl.IsNew());

//        //    return modl;
//        //}

//        //public T GetModl<T>(object id, Database database) where T : IDbModl, new()
//        //{
//        //    T modl = DbModl<T>.Get(id, database);

//        //    if (modl != null)
//        //        Assert.True(!modl.IsNew());

//        //    return modl;
//        //}

//        //public void SwitchStaticDatabaseAndCRUD(string databaseName)
//        //{
//        //    DbModl<Car>.DefaultDatabase = Database.Get(databaseName);
//        //    Assert.Equal(databaseName, DbModl<Car>.DefaultDatabase.Name);

//        //    CRUD();

//        //    DbModl<Car>.DefaultDatabase = null;
//        //}

//        //public void SwitchInstanceDatabaseAndCRUD(string databaseName)
//        //{
//        //    CRUD(Database.Get(databaseName));
//        //}

//        //public void GetFromDatabaseProvider(string databaseName)
//        //{
//        //    var db = Database.Get(databaseName);

//        //    var car = DbModl<Car>.New(db); // db.New<Car, int>();
//        //    car.Manufacturer = new Manufacturer("Saab");
//        //    car.Name = "9000";
//        //    car.WriteToDb();

//        //    var car2 = DbModl<Car>.Get(car.Id, db); // db.Get<Car, int>(car.Id);
//        //    AssertEqual(car, car2);

//        //    car2.DeleteFromDb();
//        //}

//        //public void GetFromLinq()
//        //{
//        //    var car = new Car();
//        //    car.Manufacturer = new Manufacturer("Saab");
//        //    car.Name = "9000";
//        //    car.WriteToDb();

//        //    var cars = DbModl<Car>.Query().Where(x => x.Id == car.Id).ToList();
//        //    Assert.Equal(1, cars.Count);

//        //    var car2 = cars.First();
//        //    AssertEqual(car, car2);
            
//        //    car2.DeleteFromDb();

//        //    Car c = new Car();
            
//        //}

//        //public void GetFromLinqInstance(string databaseName)
//        //{
//        //    var db = Database.Get(databaseName);

//        //    var car = DbModl<Car>.New(db); //db.New<Car, int>();
//        //    car.Manufacturer = new Manufacturer("Saab");
//        //    car.Name = "9000";
//        //    car.WriteToDb();

//        //    var cars = DbModl<Car>.Query(db).Where(x => x.Id == car.Id).ToList();
//        //    Assert.Equal(1, cars.Count);

//        //    var selectList = DbModl<Car>.Query(db).Where(x => x.Name != "dsklhfsd").AsEnumerable().AsSelectList(x => x.Manufacturer.Name + " " + x.Name);
//        //    Assert.True(selectList.Count() > 0);
            
//        //    //var car2 = cars.First();
//        //    var car2 = DbModl<Car>.Query(db).Where(x => x.Id == car.Id).First();
//        //    AssertEqual(car, car2);

//        //    var car3 = DbModl<Car>.GetWhere(x => x.Name == "9000", db);
//        //    Assert.Equal("9000", car3.Name);

//        //    car2.DeleteFromDb();
//        //}

//        //public void GetFromLinqAdvanced(string databaseName)
//        //{
//        //    var db = Database.Get(databaseName);

//        //    var car = DbModl<Car>.New(db);
//        //    car.Manufacturer = new Manufacturer("Saab");
//        //    car.Name = "9000";
//        //    car.WriteToDb();

//        //    var cars = DbModl<Car>.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").ToList();
//        //    Assert.Equal(1, cars.Count);

//        //    var car2 = DbModl<Car>.Query(db).Where(x => x.Id == car.Id && x.Manufacturer == car.Manufacturer && x.Name != "M5").First();
//        //    AssertEqual(car, car2);
            
//        //    car2.DeleteFromDb();
//        //}

//        //public void StaticDelete()
//        //{
//        //    var cars = NewCars(5);
//        //    Assert.True(DbModl<Car>.GetAll().Count() >= cars.Count);

//        //    DbModl<Car>.DeleteAll();
//        //    Assert.Equal(0, DbModl<Car>.GetAll().Count());

//        //    cars = NewCars(5);
//        //    Assert.Equal(5, DbModl<Car>.GetAll().Count());

//        //    DbModl<Car>.Delete(cars[0].Id);
//        //    Assert.False(DbModl<Car>.Exists(cars[0].Id));
//        //    Assert.Equal(4, DbModl<Car>.GetAll().Count());

//        //    cars[1].Name = "10000";
//        //    cars[1].WriteToDb();
//        //    DbModl<Car>.DeleteAllWhere(x => x.Name == "9000");
//        //    Assert.True(DbModl<Car>.Exists(cars[1].Id));
//        //    Assert.Equal(1, DbModl<Car>.GetAll().Count());

//        //    cars[1].DeleteFromDb();
//        //    Assert.Equal(0, DbModl<Car>.GetAll().Count());
//        //}

//        //public List<Car> NewCars(int count, bool save = true)
//        //{
//        //    List<Car> list = new List<Car>(count);

//        //    while (count-- > 0)
//        //    {
//        //        var car = new Car();
//        //        car.Manufacturer = new Manufacturer("Saab");
//        //        car.Name = "9000";

//        //        if (save)
//        //            car.WriteToDb();

//        //        list.Add(car);
//        //    }

//        //    return list;
//        //}

//        //public void AssertEqual(Car car1, Car car2)
//        //{
//        //    Assert.Equal(car1.Database(), car2.Database());
//        //    Assert.Equal(car1.Database().Name, car2.Database().Name);
//        //    Assert.Equal(car1.Id, car2.Id);
//        //    Assert.Equal(car1.Manufacturer, car2.Manufacturer);
//        //    Assert.Equal(car1.Name, car2.Name);
//        //}

//        //public void AssertEqual(Manufacturer m1, Manufacturer m2)
//        //{
//        //    Assert.Equal(m1.Database(), m2.Database());
//        //    Assert.Equal(m1.Database().Name, m2.Database().Name);
//        //    Assert.Equal(m1.GetId(), m2.GetId());
//        //    Assert.Equal(m1.Name, m2.Name);
//        //}

//        //public void SetIdExplicit()
//        //{
//        //    var id = Guid.NewGuid();
//        //    Manufacturer m1 = DbModl<Manufacturer>.New(id);
//        //    m1.Name = "Audi";
//        //    Assert.Equal(id, m1.GetId());
//        //    m1.WriteToDb();
//        //    Assert.Equal(id, m1.GetId());

//        //    var m2 = DbModl<Manufacturer>.Get(m1.GetId());
//        //    AssertEqual(m1, m2);

//        //    m2.WriteToDb();
//        //    Assert.Equal(id, m2.GetId());

//        //    m2.DeleteFromDb();
//        //}

//        //public void GetAllAsync()
//        //{
//        //    //DbModl<Car>.DeleteAll();

//        //    //NewCars(10);
//        //    ////Thread.Sleep(100);

//        //    //Assert.Equal(10, DbModl<Car>.GetAll().Count());

//        //    //List<Task<Car>> carsAsync = new List<Task<Car>>();
//        //    //var cars = DbModl<Car>.GetAll().ToList();
//        //    //foreach (var car in cars)
//        //    //    carsAsync.Add(Car.GetAsync(car.Id));

//        //    //for (int i = 0; i < cars.Count; i++)
//        //    //{
//        //    //    AssertEqual(cars[i], carsAsync[i].Result);
//        //    //    carsAsync[i].Result.DeleteFromDb();
//        //    //}

//        //    //Assert.Equal(0, DbModl<Car>.GetAll().Count());
//        //}
//    }
//}
