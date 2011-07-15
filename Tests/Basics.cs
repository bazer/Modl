using ExampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.DatabaseProviders;
using System.Linq;

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

        public void SwitchDatabase(string databaseName)
        {
            Database.Default = Database.Get(databaseName);

            Assert.AreEqual(databaseName, Database.Default.Name);
            Assert.AreEqual(Database.Default, Car.DefaultDatabase);
            Assert.AreEqual(Database.Default, Database.Get(Car.New().DatabaseName));
        }

        public void CRUD(Database database = null)
        {
            Car car = NewModl<Car>(database);
            Assert.AreEqual(false, car.IsDirty);
            car.Name = "BMW M3";
            car.Manufacturer = "BMW";
            Assert.AreEqual(true, car.IsDirty);
            car.Save();
            Assert.IsTrue(!car.IsNew);
            Assert.AreEqual(false, car.IsDirty);

            Car car2 = GetModl<Car>(car.Id, database); // Car.Get(car.Id);
            Assert.AreEqual(car.Id, car2.Id);
            Assert.AreEqual(car.Name, car2.Name);
            Assert.AreEqual(car.Manufacturer, car2.Manufacturer);

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
            Car.SetDefaultDatabase(databaseName);
            Assert.AreEqual(databaseName, Car.DefaultDatabase.Name);

            CRUD();

            Car.ClearDefaultDatabase();
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
            Assert.AreEqual(car.Database, car2.Database);
            Assert.AreEqual(car.DatabaseName, car2.DatabaseName);
            Assert.AreEqual(car.Manufacturer, car2.Manufacturer); 
            Assert.AreEqual(car.Name, car2.Name);

            //db.Select<Car>().Where(x => x.Id == car.Id).ToList();

            //db.Get<Car>(car.Id);
        }
    }
}
