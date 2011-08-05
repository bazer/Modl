using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Modl.Linq
{
    internal class LinqQueryProvider<M> : IQueryProvider
         where M : Modl<M>, new()
    {
        Database database;

        public LinqQueryProvider(Database database)
        { 
            this.database = database;
        }

        public IQueryable<T> CreateQuery<T>(System.Linq.Expressions.Expression expression)
        {
            return (IQueryable<T>)new LinqQuery<M>(database, expression);

           // return CreateNewQuery<T>(expression);    
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return new LinqQuery<M>(database, expression);
        }

        public T Execute<T>(System.Linq.Expressions.Expression expression)
        {
            return Helper.ConvertTo<T>(Modl<M>.Get(new Query.Select<M>(database, expression)));
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return Modl<M>.Get(new Query.Select<M>(database, expression));
        }

        //return new LinqQuery<T>(database, expression);
        //protected IQueryable<T> CreateNewQuery<T>(Expression expression)
        //{
        //    return (IQueryable<T>)Activator.CreateInstance(typeof(LinqQuery<>).MakeGenericType(typeof(T)), BindingFlags.Instance | BindingFlags.Public, null, new object[] { database, expression }, null);
        //}
    }
}
