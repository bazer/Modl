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
using System.Linq.Expressions;
using Modl.Linq.Parsers;

namespace Modl.Query
{
    public class Delete<M, IdType> : Query<M, IdType, Delete<M, IdType>>
        where M : Modl<M, IdType>, new()
    {
        Expression expression;

        public Delete(Database database) : base(database) { }

        public Delete(Database database, Expression expression)
            : base(database)
        {
            this.expression = expression;
            var parser = new LinqParser<M, IdType, Delete<M, IdType>>(this);
            parser.ParseTree(expression);
        }

        public override Sql ToSql(string paramPrefix)
        {
            //var sql = new Sql().AddFormat("DELETE FROM {0} \r\n", Modl<M, IdType>.Table);

            return GetWhere(
                new Sql().AddFormat("DELETE FROM {0} \r\n", Modl<M, IdType>.Table), 
                paramPrefix);


            //var where = GetWhere(sql, paramPrefix);

            //return where.

            //return new Sql(
            //    string.Format("DELETE FROM {0} \r\n{1}", Modl<M, IdType>.Table, where.Text),
            //    where.Parameters);
        }

        public override int ParameterCount
        {
            get { return whereList.Count; }
        }

        //public override string ToString()
        //{
        //    return string.Format("DELETE FROM {0} \r\nWHERE \r\n{1}", Modl<C>.TableName, QueryPartsToString());
        //}
    }
}
