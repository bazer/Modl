﻿/*
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
using Modl.Structure;
using Modl.Structure.Metadata;

namespace Modl
{
    public class Modl<M>
        where M : IModl, new()
    {
        public static Settings Settings { get { return Settings.Get(typeof(M)); } }

        public static Metadata Metadata { get { return Metadata.Get(typeof(M)); } }

        static Modl()
        {
        }

        public static M New()
        {
            return new M().Modl();
            //return New(Guid.NewGuid().ToString());
            

            //return new M().Modl();
        }

        public static M New(string id)
        {
            return new M().Modl().SetId(id);
            //m.ModlData.Id = id;

            //return m;

            //var data = new ModlData
            //{
            //    Id = id
            //};

            //return new M()
            //{
            //    ModlData = data  
            //}
            //.Modl(); 
        }

        public static M Get(string id)
        {
            return Internal.Get<M>(id);
        }
    }
}
