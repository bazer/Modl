using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExampleModel;

namespace Tests
{
    [TestClass]
    public class Basics
    {
        public void CRUD()
        {
            Car car = new Car
            {
                Name = "BMW M3",
                Manufacturer = "BMW"
            };

            Assert.IsTrue(car.IsNew);
            car.Save();
            Assert.IsTrue(!car.IsNew);

            Car car2 = Car.Get(car.Id);
            Assert.IsTrue(!car2.IsNew);
            Assert.AreEqual(car.Id, car2.Id);
            Assert.AreEqual(car.Name, car2.Name);
            Assert.AreEqual(car.Manufacturer, car2.Manufacturer);

            car2.Manufacturer = "Mercedes";
            Assert.AreEqual("Mercedes", car2.Manufacturer);
            car2.Save();

            Car car3 = Car.Get(car.Id);
            Assert.AreEqual("Mercedes", car3.Manufacturer);
            car3.Delete();
            Assert.IsTrue(car3.IsDeleted);
            Assert.AreEqual(null, Car.Get(car.Id, false));
        }

        public void SwitchDatabase(string databaseName)
        {
            Modl.Config.DefaultDatabase = Modl.Config.GetDatabase(databaseName);

            Assert.AreEqual(databaseName, Modl.Config.DefaultDatabase.Name);
            Assert.AreEqual(Modl.Config.DefaultDatabase, Modl.Config.GetDatabase(Car.DatabaseName));
            Assert.AreEqual(Modl.Config.DefaultDatabase, Car.Database);
        }
    }
}
