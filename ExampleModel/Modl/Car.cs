/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
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
    [Cache(CacheLevel.Off, CacheTimeout.Never)]
    public class Car: IModl //: Vehicle
    {
        //[ForeignKey(typeof(Vehicle))]
        //public int Vehicle_fk { get; set; }
        public IModlData Modl { get; set; }

        [Key(automatic: true)]
        public string Id { get; set; }
        [Name("CarName")]
        public string Name { get; set; }
        [Name("Type_fk")]
        public CarType Type { get; set; }
        public List<string> Tags { get; set; }

        public Manufacturer Manufacturer { get { return this.GetRelation<Car, Manufacturer>(nameof(Manufacturer)); } set { this.SetRelation(nameof(Manufacturer), value); } }

        //public string GetId() => Id.ToString();
        //public void SetId(string id) => Id = int.Parse(id);
    }
}
