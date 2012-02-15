using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;

namespace ExampleModel
{
    public class ModlContext
    {
        Database db;

        public ModlContext(Database database)
        {
            this.db = database;
        }

        public ModlContext(string databaseName)
        {
            this.db = Database.Get(databaseName);
        }

        public Vehicle GetVehicle(int id)
        {
            return Modl<Vehicle>.New();
        }

        public bool GetCar(int id)
        {
            var car = new Car();
            car.IsNew();
            car.IsNewText();

            var man = new Manufacturer();
            man.WriteToDb();

            return TxtModl<Car>.New().IsNewText();
        }

        public IQueryable<Car> Cars
        {
            get 
            {
                return DbModl<Car>.Query(db);
            }
        }

        public IQueryable<Manufacturer> Manufacturers
        {
            get { return DbModl<Manufacturer>.Query(db); }
        }
    }
}
