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
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public interface IQuery
    {
        Database DatabaseProvider { get; }
        IDbCommand ToDbCommand();
        Tuple<string, IEnumerable<IDataParameter>> ToSql();
        //Where<C, T> Where(string key);
        //IEnumerable<IDataParameter> QueryPartsParameters();
    }

    public abstract class Query<M, Q> : IQuery
        where M : Modl<M>, new()
        where Q : Query<M, Q>
    {
        protected List<Where<M, Q>> whereList = new List<Where<M, Q>>();
        protected ModlBase owner;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }
        public abstract Tuple<string, IEnumerable<IDataParameter>> ToSql();

        public Query()
        {
        }

        public Query(Database database)
        {
            provider = database;
        }

        public Query(M owner)
        {
            this.owner = owner;
        }

        public Where<M, Q> Where(string key)
        {
            var where = new Where<M, Q>((Q)this, key);
            whereList.Add(where);

            return where;
        }

        protected Tuple<string, IEnumerable<IDataParameter>> GetWhere()
        {
            if (whereList.Count == 0)
                return new Tuple<string, IEnumerable<IDataParameter>>(string.Empty, new List<IDataParameter>());

            int i = 0, j = 0;
            return new Tuple<string, IEnumerable<IDataParameter>>("WHERE \r\n" +
                string.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(i++))),
                whereList.Select(x => x.GetCommandParameter(j++)));
        }

        //protected string QueryPartsToString()
        //{
        //    int i = 0;
        //    return string.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(i++)));
        //}

        //public IEnumerable<IDataParameter> QueryPartsParameters()
        //{
        //    int i = 0;
        //    return whereList.Select(x => x.GetCommandParameter(i++));
        //}

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }
    }
}
