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
