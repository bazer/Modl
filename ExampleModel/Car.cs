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

        public Car GetFromName(string name)
        {
            return Get(
                Select()
                .Where("Name").EqualTo(name)
                .And(
                     Where("Manufacturer").Like("Merc%")
                     .Or()
                     .Where("Manufacturer").Like("%edes")
                    )
            );
            

            //return null;
        }
    }
}
