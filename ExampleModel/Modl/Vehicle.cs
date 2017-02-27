using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;
using Modl.Structure;

namespace ExampleModel
{
    [Name("Vehicles")]
    public interface IVehicle : IModl
    {
        [Id(automatic: true)]
        string Id { get; set; }

        [Name("Manufacturer_fk")]
        IManufacturer Manufacturer { get; set; }
    }

    [Name("Vehicles")]
    [Cache(CacheLevel.Off, CacheTimeout.Never)]
    public class Vehicle : IModl
    {
        public IModlData Modl { get; set; }

        [Id(automatic: true)]
        public string Id { get; set; }

        [Name("Manufacturer_fk")]
        public ModlValue<Manufacturer> Manufacturer { get; set; }

        public bool IsMutable => throw new NotImplementedException();
        //public Manufacturer Manufacturer { get { return this.GetRelation<Vehicle, Manufacturer>(nameof(Manufacturer)); } set { this.SetRelation(nameof(Manufacturer), value); } }
    }
}
