using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;

namespace ExampleModel
{
    [Name("Manufacturers")]
    public class Manufacturer : IModl
    {
        public IModlData Modl { get; set; }

        [Id]
        public Guid ManufacturerID { get; set; }
        public string Name { get; set; }

        [ForeignKey(typeof(Vehicle), "Manufacturer_fk")]
        public List<Vehicle> Vehicles { get; set; }
        //public List<Vehicle> Vehicles { get { return this.GetFk("Manufacturer"); } set { SetFk(value); } }


        public Manufacturer() { }
        public Manufacturer(string name)
        {
            Name = name;
            ManufacturerID = Guid.NewGuid();
            //this.Modl();
        }
    }
}
