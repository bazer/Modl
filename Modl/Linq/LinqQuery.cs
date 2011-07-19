using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using Modl.Query;

namespace Modl.Linq
{
    public class LinqQuery<T> : IQueryable<T> where T : Modl<T>, new()
    {
        Database database;
        Expression expression;
        IQueryProvider provider;

        public LinqQuery(Database database)
        { 
            this.database = database;
            this.expression = Expression.Constant(this);
            this.provider = new LinqQueryProvider(database);
        }

        public LinqQuery(Database database, Expression expression)
        {
            this.database = database;
            this.expression = expression;
            this.provider = new LinqQueryProvider(database);
        }

        protected IEnumerator<T> GetList()
        {
            return Modl<T>.GetList(new Select<T>(database, expression)).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetList();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetList();
        }

        public Type ElementType
        {
            get { return typeof(T); }
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
