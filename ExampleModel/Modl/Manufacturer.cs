using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;

namespace ExampleModel
{
    [Name("Manufacturers")]
    public interface IManufacturer : IMutable
    {
        [Id]
        Guid ManufacturerID { get; }
        string Name { get; }
        ModlCollection<Car> Cars { get; }

        [ForeignKey(typeof(Vehicle), "Manufacturer_fk")]
        ModlCollection<Vehicle> Vehicles { get; }
        string Item { get; set; }
    }

    [Name("Manufacturers")]
    public class Manufacturer : IModl
    {
        public IModlData Modl { get; set; }

        [Id]
        public Guid ManufacturerID { get; set; }
        public string Name { get; set; }
        public ModlCollection<Car> Cars { get; set; }

        [ForeignKey(typeof(Vehicle), "Manufacturer_fk")]
        public ModlCollection<Vehicle> Vehicles { get; set; }

        public bool IsMutable => throw new NotImplementedException();

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
