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
    public class Update<M, IdType> : Change<M, IdType>
        where M : Modl<M, IdType>, new()
    {
        public Update(Database database) : base(database) { }

        protected Sql GetWith(Sql sql, string paramPrefix)
        {
            int length = withList.Count;
            if (length == 0)
                return sql;
            
            int i = 0;
            foreach (var with in withList)
            {
                DatabaseProvider.GetParameter(sql, paramPrefix + "v" + i, with.Value);
                DatabaseProvider.GetParameterComparison(sql, with.Key, Relation.Equal, paramPrefix + "v" + i);

                if (i + 1 < length)
                    sql.AddText(" AND \r\n");

                i++;
            }

            return sql;

            //int i = 0, j = 0;

            //return sql
            //    .Join(",", withList.Select(x => DatabaseProvider.GetParameterComparison(x.Key, Relation.Equal, paramPrefix + "v" + i++)).ToArray())
            //    .AddParameters(withList.Select(x => DatabaseProvider.GetParameter(paramPrefix + "v" + j++, x.Value)).ToArray());

            //return new Sql(
            //    string.Join(",", withList.Select(x => DatabaseProvider.GetParameterComparison(x.Key, Relation.Equal, paramPrefix + "v" + i++))),
            //    withList.Select(x => DatabaseProvider.GetParameter(paramPrefix + "v" + j++, x.Value)).ToArray());
        }

        public override Sql ToSql(string paramPrefix)
        {
            var sql = GetWith(
                new Sql().AddFormat("UPDATE {0} SET ", Modl<M, IdType>.Table),
                paramPrefix);

            return GetWhere(
                sql.AddText(" \r\n"),
                paramPrefix);

            //var with = GetWith(paramPrefix);
            //var where = GetWhere(paramPrefix);

            //return new Sql(
            //    string.Format("UPDATE {0} SET {1} \r\n{2}", Modl<M, IdType>.Table, with.Text, where.Text),
            //    with.Parameters.Concat(where.Parameters).ToArray());
        }

        public override int ParameterCount
        {
            get { return withList.Count + whereList.Count; }
        }
    }
}
