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

namespace Modl.Structure
{
    public class ModlProperty
    {
        protected object oldValue;
        protected object newValue;
        protected bool isDirty = false;
        internal bool IsDirty { get { return isDirty; } }
        internal Type Type { get; set; }
        //protected bool emptyProperty;

        internal ModlProperty(object value, Type type)
        {
            oldValue = value;
            newValue = value;

            Type = type;
            //this.emptyProperty = emptyProperty;
        }

        internal object Value
        {
            get
            {
                return newValue;
            }
            set
            {
                newValue = value;
                isDirty = !object.Equals(oldValue, newValue);
                //isDirty = oldValue != newValue;
                //isDirty = !EqualityComparer<T>.Default.Equals(oldValue, newValue);
            }
        }

        internal void Reset()
        {
            oldValue = newValue;
            isDirty = false;
        }
    }
}
