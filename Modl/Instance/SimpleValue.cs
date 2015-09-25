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

namespace Modl.Structure.Instance
{
    public interface IValue
    {
        bool IsModified { get; }
        void Set(object value);
        object Get();
        void Reset();
    }

    public class SimpleValue : IValue
    {
        public bool IsModified { get; private set; }

        protected object oldValue;
        protected object newValue;

        public SimpleValue(object value)
        {
            oldValue = value;
            newValue = value;
        }

        public void Set(object value)
        {
            newValue = value;
            IsModified = !object.Equals(oldValue, newValue);
        }

        public object Get()
        {
            return newValue;
        }

        public void Reset()
        {
            oldValue = newValue;
            IsModified = false;
        }
    }
}
