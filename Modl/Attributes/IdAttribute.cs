﻿/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

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

namespace Modl
{
    public enum IdType
    {
        Int,
        Guid
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class IdAttribute : Attribute
    {
        public string Name { get; private set; }
        public IdType Type { get; private set; }
        
        public IdAttribute(string name, IdType type = IdType.Int)
        {
            Name = name;
            Type = type;
        }
    }
}
