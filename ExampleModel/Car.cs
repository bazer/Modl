using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;

namespace ExampleModel
{
    public class Car : Modl<Car>
    {
        static Car()
        {
            TableName = "Cars";
        }

        public string Name { get { return Fields.Name; } set { Fields.Name = value; } }
        public string Manufacturer { get { return F.Manufacturer; } set { F.Manufacturer = value; } }

        public static Car GetFromName(string name)
        {
            Query().Where(x => x.Name == name).Single();

            //return Get(
            //    Select().Where(x => x.Name == name).Where(x => x.Manufacturer.StartsWith("Merc") || x.Manufacturer.EndsWith("edes"));

            //    //Select()
            //    //.Where("Name").EqualTo(name)
            //    //.And(
            //    //     Where("Manufacturer").Like("Merc%")
            //    //     .Or()
            //    //     .Where("Manufacturer").Like("%edes")
            //    //    )
            //);

            var c = from t in Query()
                    where t.Name == name
                    select t;

            return c.Single();

        }
    }
}
