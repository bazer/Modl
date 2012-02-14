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
using System.Reflection;

namespace Modl.Linq
{
    internal class LinqQueryProvider<M> : IQueryProvider
         where M : IDbModl<M>, new()
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
            return Helper.ConvertTo<T>(new Query.Select<M>(database, expression).GetAsync().Result);
            //return Helper.ConvertTo<T>(Modl<M>.Get(new Query.Select<M>(database, expression)));
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return new Query.Select<M>(database, expression).GetAsync().Result;
        }

        //return new LinqQuery<T>(database, expression);
        //protected IQueryable<T> CreateNewQuery<T>(Expression expression)
        //{
        //    return (IQueryable<T>)Activator.CreateInstance(typeof(LinqQuery<>).MakeGenericType(typeof(T)), BindingFlags.Instance | BindingFlags.Public, null, new object[] { database, expression }, null);
        //}
    }
}
