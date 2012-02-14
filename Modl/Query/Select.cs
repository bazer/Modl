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
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Modl.Linq.Parsers;
using System.Data.Common;
using Modl.DataAccess;
using Modl.Fields;
using System.Threading.Tasks;

namespace Modl.Query
{
    public class Select<M> : Query<M, Select<M>>
        where M : IDbModl<M>, new()
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

        public override Sql ToSql(string paramPrefix)
        {
            return GetWhere(
                new Sql().AddFormat("SELECT * FROM {0} \r\n", DbModl<M>.Table),
                paramPrefix);


            //var where = GetWhere(paramPrefix);

            //return new Sql(
            //    string.Format("SELECT * FROM {0} \r\n{1}", Modl<M, IdType>.Table, where.Text),
            //    where.Parameters);
        }

        

        public override int ParameterCount
        {
            get { return whereList.Count; }
        }

        public DbDataReader Execute()
        {
            return DbAccess.ExecuteReader(this).First();
        }

        public M Get()
        {
            return new Materializer<M>(Execute(), DatabaseProvider).ReadAndClose();
        }

        public IEnumerable<M> GetAll()
        {
            return new Materializer<M>(Execute(), DatabaseProvider).GetAll();
        }

        public Materializer<M> GetMaterializer()
        {
            return new Materializer<M>(Execute(), DatabaseProvider);
        }

        public Task<DbDataReader> ExecuteAsync(bool onQueue = true)
        {
            return AsyncDbAccess.ExecuteReader(this, onQueue);
        }

        public Task<M> GetAsync(bool onQueue = true)
        {
            return Materializer<M>.Async(ExecuteAsync(onQueue), DatabaseProvider).ContinueWith(x => x.Result.ReadAndClose());
        }

        public Task<IEnumerable<M>> GetAllAsync(bool onQueue = true)
        {
            return Materializer<M>.Async(ExecuteAsync(onQueue), DatabaseProvider).ContinueWith(x => x.Result.GetAll());
        }

        public Task<Materializer<M>> GetMaterializerAsync(bool onQueue = true)
        {
            return Materializer<M>.Async(ExecuteAsync(onQueue), DatabaseProvider);
        }
    }
}
