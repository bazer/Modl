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

namespace Modl.Query
{
    public class Literal : IQuery
    {
        private string sql;
        IEnumerable<IDataParameter> parameters;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }

        public Literal(Database database, string sql)
        {
            this.provider = database;
            this.sql = sql;
            this.parameters = new List<IDataParameter>();
        }

        public Literal(Database database, string sql, IEnumerable<IDataParameter> parameters)
        {
            this.provider = database;
            this.sql = sql;
            this.parameters = parameters;
        }

        public Sql ToSql(string paramPrefix)
        {
            return new Sql(sql, parameters.ToArray());
        }

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }

        public int ParameterCount
        {
            get { return parameters.Count(); }
        }

        //public IEnumerable<IDataParameter> QueryPartsParameters()
        //{
        //    return new List<IDataParameter>();
        //}
    }
}
