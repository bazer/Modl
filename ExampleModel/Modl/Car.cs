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

namespace ExampleModel
{
    [Table("Cars")]
    //[Id("Id")]
    [Cache(CacheLevel.Off, Timeout.Never)]
    public class Car : IDbModl<Car>, ITxtModl<Car>, IMvcModl<Car>
    {
        [Id("Id")]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Manufacturer { get; set; }
        //public string Manufacturer { get { return GetValue<string>("Manufacturer"); } set { SetValue("Manufacturer", value); } }
    }
}
