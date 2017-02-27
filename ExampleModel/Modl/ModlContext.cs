using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;
//using Modl.Db;

namespace ExampleModel
{
    public class ModlContext
    {
        public class ImmutableContext<T> where T : class, IModl
        {
            public T Get(object id) => M.Get<T>(id);
        }

        public class MutableContext<T> : ImmutableContext<T> where T : class, IMutable
        {
            public T New() => M.New<T>();
        }

        public static MutableContext<ICar> Car => new MutableContext<ICar>();
        public static MutableContext<IManufacturer> Manufacturer => new MutableContext<IManufacturer>();
        public static ImmutableContext<IVehicle> Vehicle => new ImmutableContext<IVehicle>();



        //Database db;

        //public ModlContext(Database database)
        //{
        //    this.db = database;
        //}

        //public ModlContext(string databaseName)
        //{
        //    this.db = Database.Get(databaseName);
        //}

        //public Vehicle GetVehicle(int id)
        //{
        //    return Modl<Vehicle>.New();
        //}

        //public bool GetCar(int id)
        //{
        //    var car = new Car();
        //    car.IsNew();
        //    car.IsNewText();

        //    var man = new Manufacturer();
        //    //man.WriteToDb();

        //    return TxtModl<Car>.New().IsNewText();
        //}

        //public IQueryable<Car> Cars
        //{
        //    get 
        //    {
        //        return DbModl<Car>.Query(db);
        //    }
        //}

        //public IQueryable<Manufacturer> Manufacturers
        //{
        //    get { return DbModl<Manufacturer>.Query(db); }
        //}
    }
}
