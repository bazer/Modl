using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;
using Modl.Structure;

namespace ExampleModel
{
    [Name("Vehicles")]
    [Cache(CacheLevel.Off, CacheTimeout.Never)]
    public class Vehicle : IModl
    {
        public IModlData Modl { get; set; }

        [Id]
        public string Id { get; set; }

        [Name("Manufacturer_fk")]
        public Manufacturer Manufacturer { get { return this.GetRelation<Vehicle, Manufacturer>(nameof(Manufacturer)); } set { this.SetRelation(nameof(Manufacturer), value); } }
    }
}
