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
using System.Data;

namespace Modl.Query
{
    public class Insert<M> : Change<M>
        where M : IDbModl<M>, new()
    {
        public Insert(Database database) : base(database) { }

        protected Sql GetWith(Sql sql, string paramPrefix)
        {
            int length = withList.Count;
            if (length == 0)
                return sql;

            sql.AddFormat("({0}) VALUES (", string.Join(",", withList.Keys));

            int i = 0;
            foreach (var with in withList)
            {
                DatabaseProvider.GetParameter(sql, paramPrefix + "v" + i, with.Value);
                DatabaseProvider.GetParameterValue(sql, paramPrefix + "v" + i);

                if (i + 1 < length)
                    sql.AddText(",");
                else
                    sql.AddText(")");

                i++;
            }

            return sql;

            //int i = 0, j = 0;

            //return sql
            //    .AddFormat("({0}) VALUES ({1})",
            //        string.Join(",", withList.Keys),
            //        string.Join(",", withList.Values.Select(x => DatabaseProvider.GetParameterValue(paramPrefix + "v" + i++))))
            //    .AddParameters(withList.Select(x => DatabaseProvider.GetParameter(paramPrefix + "v" + j++, x.Value)).ToArray());

            //return new Sql(
            //    string.Format("({0}) VALUES ({1})",
            //        string.Join(",", withList.Keys),
            //        string.Join(",", withList.Values.Select(x => DatabaseProvider.GetParameterValue(paramPrefix + "v" + i++)))),
            //    withList.Select(x => DatabaseProvider.GetParameter(paramPrefix + "v" + j++, x.Value)).ToArray());

            //return string.Format("({0}) VALUES ({1})",
            //    string.Join(",", withList.Keys),
            //    string.Join(",", withList.Values.Select(x => "'" + x + "'"))
            //);
        }

        public override Sql ToSql(string paramPrefix)
        {
            return GetWith(
                new Sql().AddFormat("INSERT INTO {0} ", DbModl<M>.Table),
                paramPrefix);

            //var with = GetWith(paramPrefix);

            //return new Sql(
            //    string.Format("INSERT INTO {0} {1}", Modl<M, IdType>.Table, with.Text),
            //    with.Parameters);
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
