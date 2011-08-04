using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;
using Modl.Attributes;

namespace ExampleModel
{
    [Table("Cars")]
    [Id("Id")]
    public class Car : Modl<Car>
    {
        public string Name { get { return Fields.Name; } set { Fields.Name = value; } }
        public string Manufacturer { get { return GetValue<string>("Manufacturer"); } set { SetValue("Manufacturer", value); } }
    }
}
