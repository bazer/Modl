using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using Modl.Query;

namespace Modl.Linq
{
    public class LinqQuery<M> : IQueryable<M>
        where M : Modl<M>, new()
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
            return Modl<M>.GetList(new Select<M>(database, expression)).GetEnumerator();
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
