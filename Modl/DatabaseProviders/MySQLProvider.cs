using System;
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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Modl.Query;
using MySql.Data.MySqlClient;

namespace Modl.DatabaseProviders
{
    public class MySQLProvider : Database
    {
        public static new DatabaseType Type { get { return DatabaseType.MySQL; } }
        internal static new string[] ProviderNames = new string[1] { "MySql.Data.MySqlClient" };

        protected MySQLProvider(string name, string connectionString, string provider) : base(name, connectionString, provider) { }

        internal override IDbConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        internal static MySQLProvider GetNewOnMatch(ConnectionStringSettings connectionConfig)
        {
            if (ProviderNames.Contains(connectionConfig.ProviderName))
                return new MySQLProvider(connectionConfig.Name, connectionConfig.ConnectionString, connectionConfig.ProviderName);

            return null;
        }

        internal override IQuery GetLastIdQuery()
        {
            return new Literal(this, "SELECT last_insert_id()");
        }

        internal override Sql GetParameterValue(Sql sql, string key)
        {
            return sql.AddFormat("?{0}", key);
        }

        internal override Sql GetParameterComparison(Sql sql, string field, Relation relation, string key)
        {
            return sql.AddFormat("{0} {1} ?{2}", field, relation.ToSql(), key);
        }

        internal override Sql GetParameter(Sql sql, string key, object value)
        {
            return sql.AddParameters(new MySqlParameter("?" + key, value ?? DBNull.Value));
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            var sql = query.ToSql("");
            var command = new MySqlCommand(sql.Text, (MySqlConnection)GetConnection());
            command.Parameters.AddRange(sql.Parameters.ToArray());

            return command;
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            int i = 0;
            var sql = queries.Select(x => x.ToSql("q" + i++)).ToList();
            var commands = new List<IDbCommand>();

            var command = new MySqlCommand(string.Join("; \r\n", sql.Select(x => x.Text)), (MySqlConnection)GetConnection());
            command.Parameters.AddRange(sql.SelectMany(x => x.Parameters).ToArray());
            commands.Add(command);

            return commands;
        }
    }
}
