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

        public Car GetCar(int id)
        {
            return DbModl<Car>.Get(id);
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
