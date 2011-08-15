/*
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
using Modl.DatabaseProviders;
using System.Data;

namespace Modl.Query
{
    public class Insert<M, IdType> : Change<M, IdType>
        where M : Modl<M, IdType>, new()
    {
        public Insert(Database database) : base(database) { }

        protected Sql GetWith(string paramPrefix)
        {
            int i = 0, j = 0;
            return new Sql(
                string.Format("({0}) VALUES ({1})",
                    string.Join(",", withList.Keys),
                    string.Join(",", withList.Values.Select(x => DatabaseProvider.GetParameterValue(paramPrefix + "v" + i++)))),
                withList.Select(x => DatabaseProvider.GetParameter(paramPrefix + "v" + j++, x.Value)).ToArray());

            //return string.Format("({0}) VALUES ({1})",
            //    string.Join(",", withList.Keys),
            //    string.Join(",", withList.Values.Select(x => "'" + x + "'"))
            //);
        }

        public override Sql ToSql(string paramPrefix)
        {
            var with = GetWith(paramPrefix);

            return new Sql(
                string.Format("INSERT INTO {0} {1}", Modl<M, IdType>.Table, with.Text),
                with.Parameters);
        }

        public override int ParameterCount
        {
            get { return withList.Count; }
        }

        //public override string ToString()
        //{
        //    return string.Format("INSERT INTO {0} {1}", Modl<C>.TableName, ValuesToString());
        //}
    }
}
