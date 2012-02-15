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
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public abstract class Change<M> : Query<M, Change<M>>
        where M : IDbModl, new()
    {
        public Change(Database database) : base(database) { }

        protected Dictionary<string, object> withList = new Dictionary<string, object>();

        public Change<M> With(string key, string value)
        {
            return With<string>(key, value);
        }

        public Change<M> With<V>(string key, V value)
        {
            withList.Add(key, value);
            return this;
        }
    }
}
