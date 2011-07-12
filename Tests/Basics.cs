using ExampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modl;
using Modl.DatabaseProviders;

namespace Tests
{
    [TestClass]
    public class Basics
    {
        public void SwitchDatabase(string databaseName)
        {
            Modl.Config.DefaultDatabase = Modl.Config.GetDatabase(databaseName);

            Assert.AreEqual(databaseName, Modl.Config.DefaultDatabase.Name);
            Assert.AreEqual(Modl.Config.DefaultDatabase, Car.DefaultDatabase);
            Assert.AreEqual(Modl.Config.DefaultDatabase, Modl.Config.GetDatabase(Car.New().DatabaseName));
        }

        public void CRUD(DatabaseProvider database = null)
        {

            Car car = NewModl<Car>(database);
            car.Name = "BMW M3";
            car.Manufacturer = "BMW";
            car.Save();
            Assert.IsTrue(!car.IsNew);


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

        public T NewModl<T>(DatabaseProvider database) where T : Modl<T>, new()
        {
            T modl;

            if (database == null)
                modl = Modl<T>.New();
            else
                modl = Modl<T>.New(database);

            Assert.IsTrue(modl.IsNew);

            return modl;
        }

        public T GetModl<T>(int id, DatabaseProvider database, bool throwExceptionOnNotFound = true) where T : Modl<T>, new()
        {
            T modl = Modl<T>.Get(id, database, throwExceptionOnNotFound);

            //if (databaseName == null)
                //modl = Modl<T>.Get(id, throwExceptionOnNotFound: throwExceptionOnNotFound);
            //else
            //    modl = Modl<T>.Get(id, Modl.Config.GetDatabase(databaseName), throwExceptionOnNotFound);

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
            CRUD(Modl.Config.GetDatabase(databaseName));
        }
    }
}
