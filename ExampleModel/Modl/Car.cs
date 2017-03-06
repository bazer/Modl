using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;
using Modl.Mvc;
using Modl.Structure;

namespace ExampleModel
{
    [Name("Cars")]
    public interface ICar : IMutable
    {
        [Id(automatic: true)]
        Guid Id { get; }
        [Name("CarName")]
        string Name { get; set; }
        [Name("Type_fk")]
        CarType Type { get; set; }
        IList<string> Tags { get; set; }
        IManufacturer Manufacturer { get; set; }
        //ModlValue<Manufacturer> Manufacturer { get; }
    }

    [Name("Cars")]
    [Cache(CacheLevel.Off, CacheTimeout.Never)]
    public class Car : IModl //: Vehicle
    {
        //[ForeignKey(typeof(Vehicle))]
        //public int Vehicle_fk { get; set; }
        public IModlData Modl { get; set; }

        [Id(automatic: true)]
        public Guid Id { get; set; }
        [Name("CarName")]
        public string Name { get; set; }
        [Name("Type_fk")]
        public CarType Type { get; set; }
        public List<string> Tags { get; set; }

        public ModlValue<Manufacturer> Manufacturer { get; set; }

        public bool IsMutable => throw new NotImplementedException();

        //public Manufacturer Manufacturer { get { return this.Relation(nameof(Manufacturer)).GetValue<Manufacturer>(); } set { this.Relation(nameof(Manufacturer)).SetValue(value); } }
        //public Manufacturer Manufacturer { get { return this.GetRelation<Car, Manufacturer>(nameof(Manufacturer)); } set { this.SetRelation(nameof(Manufacturer), value); } }
    }
}
