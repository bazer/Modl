using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Modl.Linq
{
    internal class LinqQueryProvider : IQueryProvider
    {
        Database database;

        public LinqQueryProvider(Database database)
        { 
            this.database = database;
        }

        public IQueryable<T> CreateQuery<T>(System.Linq.Expressions.Expression expression)
        {
            return CreateNewQuery<T>(expression);    
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        //return new LinqQuery<T>(database, expression);
        protected IQueryable<T> CreateNewQuery<T>(Expression expression)
        {
            return (IQueryable<T>)Activator.CreateInstance(typeof(LinqQuery<>).MakeGenericType(typeof(T)), BindingFlags.Instance | BindingFlags.Public, null, new object[] { database, expression }, null);
        }
    }
}
