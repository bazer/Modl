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
using System.Data.Common;
using Modl.DataAccess;
using Modl.Fields;

namespace Modl.Query
{
    public class Select<M, IdType> : Query<M, IdType, Select<M, IdType>>
        where M : Modl<M, IdType>, new()
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
            var parser = new LinqParser<M, IdType, Select<M, IdType>>(this);
            parser.ParseTree(expression);
        }

        public override Sql ToSql(string paramPrefix)
        {
            var where = GetWhere(paramPrefix);

            return new Sql(
                string.Format("SELECT * FROM {0} \r\n{1}", Modl<M, IdType>.Table, where.Text),
                where.Parameters);
        }

        public override int ParameterCount
        {
            get { return whereList.Count; }
        }

        public DbDataReader Execute()
        {
            return AsyncDbAccess.ExecuteReader(this);
        }

        internal M Get()
        {
            //if (StaticCache<M, )
            return Modl<M, IdType>.Get(Execute(), DatabaseProvider, true);
        }

        
        

        //public static C GetCached(int id, bool throwExceptionOnNotFound = true)
        //{
        //    if (throwExceptionOnNotFound)
        //        return AllCached.Single(x => x.Id == id);
        //    else
        //        return AllCached.SingleOrDefault(x => x.Id == id);
        //}

        //internal IEnumerable<M> GetList<IdType>()
        //{
        //    using (DbDataReader reader = Execute())
        //    {
        //        while (!reader.IsClosed)
        //        {
        //            var c = Modl<M, IdType>.Get(reader, DatabaseProvider, singleRow: false);

        //            if (c != null)
        //                yield return c;
        //        }
        //    }
        //}

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
