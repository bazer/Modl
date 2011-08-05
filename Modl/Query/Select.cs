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
using System.Data;
using System.Linq.Expressions;
using Modl.Linq.Parsers;

namespace Modl.Query
{
    public class Select<M> : Query<M, Select<M>>
        where M : Modl<M>, new()
    {
        Expression expression;

        public Select(Database database)
            : base(database)
        {
            expression = Expression.Constant(this);
        }

        public Select(Database database, Expression expression)
            : base(database)
        {
            this.expression = expression;
            var parser = new LinqParser<M, Select<M>>(this);
            parser.ParseTree(expression);
        }

        public override Tuple<string, IEnumerable<IDataParameter>> ToSql()
        {
            var where = GetWhere();

            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Format("SELECT * FROM {0} \r\n{1}", Modl<M>.Table, where.Item1),
                where.Item2);
        }


        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.AppendFormat("SELECT * FROM {0} \r\n", Modl<C>.TableName);

            

        //    if (whereList.Count > 0)
        //        sb.AppendFormat("WHERE\r\n {0}", QueryPartsToString());

        //    return sb.ToString();
        //}
    }
}
