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

namespace Modl.Fields
{
    public class Table
    {
        internal string Name { get; set; }
        internal Type Type { get; set; }

        internal Dictionary<string, Type> Fields = new Dictionary<string, Type>();
        internal Dictionary<string, Type> Keys = new Dictionary<string, Type>();
        internal Dictionary<string, Type> ForeignKeys = new Dictionary<string, Type>();

        internal bool HasKey
        {
            get
            {
                return Keys.Count != 0;
            }
        }

        internal KeyValuePair<string, Type> PrimaryKey
        {
            get
            {
                if (Keys.Count != 0)
                    return Keys.First();
                else if (ForeignKeys.Count != 0)
                    return ForeignKeys.First();

                throw new Exception("Table " + Name + " has no primary key");
            }
        }

        internal string PrimaryKeyName
        {
            get
            {
                return PrimaryKey.Key;
            }
        }

        internal Type PrimaryKeyType
        {
            get
            {
                return PrimaryKey.Value;
            }
        }
    }
}
