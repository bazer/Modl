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
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;
using Modl.Fields;

namespace Modl.Query
{
    public interface IQuery
    {
        Database DatabaseProvider { get; }
        IDbCommand ToDbCommand();
        Sql ToSql(string paramPrefix);
        int ParameterCount { get; }
        //Where<C, T> Where(string key);
        //IEnumerable<IDataParameter> QueryPartsParameters();
    }

    public abstract class Query<Q> : IQuery
        //where M : IDbModl, new()
        where Q : Query<Q>
    {
        protected List<Where<Q>> whereList = new List<Where<Q>>();
        protected IModl owner;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }
        public abstract Sql ToSql(string paramPrefix);
        public abstract int ParameterCount { get; }
        protected Table table;
        

        public Query()
        {
        }

        public Query(Database database, Table table)
        {
            this.provider = database;
            this.table = table;
        }

        public Query(IModl owner)
        {
            this.owner = owner;
        }

        public Where<Q> Where(string key)
        {
            var where = new Where<Q>((Q)this, key);
            whereList.Add(where);

            return where;
        }

        //public Q WhereNotAny(IEnumerable<IModl> collection)
        //{
        //    foreach (var m in collection)
        //        Where(table.IdName).NotEqualTo(m.GetId());

        //    return (Q)this;
        //}

        public Q WhereNotAny(IEnumerable<object> collection)
        {
            foreach (var id in collection)
                Where(table.PrimaryKeyName).NotEqualTo(id);

            return (Q)this;
        }

        protected Sql GetWhere(Sql sql, string paramPrefix)
        {
            int length = whereList.Count;
            if (length == 0)
                return sql; 

            sql.AddText("WHERE \r\n");
            
            for (int i=0; i<length; i++)
            {
                whereList[i].GetCommandParameter(sql, paramPrefix, i);
                whereList[i].GetCommandString(sql, paramPrefix, i);

                if (i + 1 < length)
                    sql.AddText(" AND \r\n");
            }

            return sql;
        }

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }

        public override string ToString()
        {
            var sql = ToSql(string.Empty);
            return sql.Text + "; " + string.Join(", ", sql.Parameters.Select(x => x.ParameterName + ": " + x.Value));
        }
    }
}
