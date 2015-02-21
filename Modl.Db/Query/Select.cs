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
using Modl.Db.Linq.Parsers;
using System.Data.Common;
using Modl.Db.DataAccess;
using Modl.Structure;
using System.Threading.Tasks;

namespace Modl.Db.Query
{
    public class Select : Query<Select>
        //where M : IDbModl, new()
    {
        Expression expression;
        protected List<Join<Select>> joinList = new List<Join<Select>>();

        public Select(Database database, ModlLayer table)
            : base(database, table)
        {
            expression = Expression.Constant(this);
        }

        public Select(Database database, ModlLayer table, Expression expression)
            : base(database, table)
        {
            this.expression = expression;
            //var parser = new LinqParser<Select>(this);
            //parser.ParseTree(expression);
        }

        public override Sql ToSql(string paramPrefix)
        {
            var sql = new Sql().AddFormat("SELECT * FROM {0} \r\n", table.Name);
            GetJoins(sql, "");
            GetWhere(sql, paramPrefix);

            return sql;

            //return GetWhere(
            //    new Sql().AddFormat("SELECT * FROM {0} \r\n", table.Name),
            //    paramPrefix);
        }

        protected Sql GetJoins(Sql sql, string tableAlias)
        {
            foreach (var join in joinList)
                join.GetSql(sql, tableAlias);

            return sql;
        }

        public Join<Select> InnerJoin(string tableName)
        {
            var join = new Join<Select>(this, tableName, JoinType.Inner);
            joinList.Add(join);

            return join;
        }

        public Join<Select> OuterJoin(string tableName)
        {
            var join = new Join<Select>(this, tableName, JoinType.Outer);
            joinList.Add(join);

            return join;
        }
        

        public override int ParameterCount
        {
            get { return whereList.Count; }
        }

        public DbDataReader Execute()
        {
            return DbAccess.ExecuteReader(this).First();
        }

        public M Get<M>() where M : IDbModl, new()
        {
            return new Materializer<M>(Execute(), DatabaseProvider).ReadAndClose();
        }

        public IEnumerable<M> GetAll<M>() where M : IDbModl, new()
        {
            return new Materializer<M>(Execute(), DatabaseProvider).GetAll();
        }

        public Materializer<M> GetMaterializer<M>() where M : IDbModl, new()
        {
            return new Materializer<M>(Execute(), DatabaseProvider);
        }

        public Task<DbDataReader> ExecuteAsync(bool onQueue = true)
        {
            return AsyncDbAccess.ExecuteReader(this, onQueue);
        }

        public Task<M> GetAsync<M>(bool onQueue = true) where M : IDbModl, new()
        {
            return Materializer<M>.Async(ExecuteAsync(onQueue), DatabaseProvider).ContinueWith(x => x.Result.ReadAndClose());
        }

        public Task<IEnumerable<M>> GetAllAsync<M>(bool onQueue = true) where M : IDbModl, new()
        {
            return Materializer<M>.Async(ExecuteAsync(onQueue), DatabaseProvider).ContinueWith(x => x.Result.GetAll());
        }

        public Task<Materializer<M>> GetMaterializerAsync<M>(bool onQueue = true) where M : IDbModl, new()
        {
            return Materializer<M>.Async(ExecuteAsync(onQueue), DatabaseProvider);
        }
    }
}
