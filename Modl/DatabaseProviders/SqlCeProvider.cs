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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using Modl.Query;

namespace Modl.DatabaseProviders
{
    public class SqlCeProvider : Database
    {
        public static new DatabaseType Type { get { return DatabaseType.SqlCE; } }
        internal static new string[] ProviderNames = new string[1] { "Microsoft.SQLSERVER.CE.OLEDB.4.0" };

        protected SqlCeProvider(string name, string connectionString, string provider) : base(name, connectionString, provider) { }

        internal override IDbConnection GetConnection()
        {
            //if (activeConnection.State != ConnectionState.Closed)
            //    activeConnection = new SqlCeConnection(ConnectionString);

            //return activeConnection;

            return new SqlCeConnection(ConnectionString);
        }

        internal static SqlCeProvider GetNewOnMatch(ConnectionStringSettings connectionConfig)
        {
            if (ProviderNames.Contains(connectionConfig.ProviderName))
                return new SqlCeProvider(connectionConfig.Name, connectionConfig.ConnectionString, connectionConfig.ProviderName);

            return null;
        }

        internal override IQuery GetLastIdQuery()
        {
            return new Literal(this, "SELECT @@IDENTITY");
        }

        internal override string GetParameterValue(string key)
        {
            return string.Format("@{0}", key);
        }

        internal override string GetParameterComparison(string field, Relation relation, string key)
        {
            return string.Format("{0} {1} @{2}", field, relation.ToSql(), key);
        }

        internal override IDataParameter GetParameter(string key, object value)
        {
            return new SqlCeParameter("@" + key, value);
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            var sql = query.ToSql("");
            var command = new SqlCeCommand(sql.Text, (SqlCeConnection)GetConnection());
            command.Parameters.AddRange(sql.Parameters);

            return command;

            //return new SqlCeCommand(query.ToString(), (SqlCeConnection)GetConnection());
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            var connection = GetConnection();
            var commands = new List<IDbCommand>();

            int i = 0;
            foreach (var query in queries)
            {
                var sql = query.ToSql("q" + i++);
                var command = new SqlCeCommand(sql.Text, (SqlCeConnection)connection);
                command.Parameters.AddRange(sql.Parameters);
                commands.Add(command);
            }

            return commands;

            //return queries.Select(x => new SqlCeCommand(x.ToString(), (SqlCeConnection)connection)).ToList<IDbCommand>();

            //List<IDbCommand> commands = new List<IDbCommand>();

            //foreach (var query in queries)
            //    commands.Add(new SqlCeCommand(query.ToString()));

            //return commands;
        }

        //internal override object Execute(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
