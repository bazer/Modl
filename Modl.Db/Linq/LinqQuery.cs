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
using System.Linq.Expressions;
using System.Collections;
using Modl.Db.Query;

namespace Modl.Db.Linq
{
    public class LinqQuery<M> : IQueryable<M>
        where M : IDbModl, new()
    {
        Database database;
        Expression expression;
        IQueryProvider provider;

        public LinqQuery(Database database)
        { 
            this.database = database;
            this.expression = Expression.Constant(this);
            this.provider = new LinqQueryProvider<M>(database);
        }

        public LinqQuery(Database database, Expression expression)
        {
            this.database = database;
            this.expression = expression;
            this.provider = new LinqQueryProvider<M>(database);
        }

        protected IEnumerator<M> GetList()
        {
            //if (expression is ConstantExpression)
            //    return Modl<M, IdType>.GetAll(database).GetEnumerator();
            //else
            //    return Modl<M, IdType>.GetAllWhere((Expression<Func<M, bool>>)expression, database).GetEnumerator();

            return new Select(database, DbModl<M>.Tables[0], expression).GetAllAsync<M>().Result.GetEnumerator();
        }

        public IEnumerator<M> GetEnumerator()
        {
            return GetList();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetList();
        }

        public Type ElementType
        {
            get { return typeof(M); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return expression; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }
    }
}
