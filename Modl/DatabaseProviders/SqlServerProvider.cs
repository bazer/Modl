using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Modl.Query;
using System.Linq.Expressions;

namespace Modl.DatabaseProviders
{
    public class SqlServerProvider : Database
    {
        public static new DatabaseType Type { get { return DatabaseType.SqlServer; } }
        internal static new string[] ProviderNames = new string[1] { "System.Data.SqlClient" };

        protected SqlServerProvider(string name, string connectionString, string provider) : base(name, connectionString, provider) { }

        internal override IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        internal static SqlServerProvider GetNewOnMatch(ConnectionStringSettings connectionConfig)
        {
            if (ProviderNames.Contains(connectionConfig.ProviderName))
                return new SqlServerProvider(connectionConfig.Name, connectionConfig.ConnectionString, connectionConfig.ProviderName);

            return null;
        }

        internal override IQuery GetLastIdQuery()
        {
            return new Literal(Name, "SELECT SCOPE_IDENTITY()");
        }

        internal override string GetCommandString(string field, Relation relation, string key)
        {
            return string.Format("{0} {1} @{2}", field, relation.ToSql(), key);
        }

        internal override IDbDataParameter GetCommandParameter(string key, object value)
        {
            return new SqlParameter("@" + key, value);
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            var command = new SqlCommand(query.ToString(), (SqlConnection)GetConnection());
            command.Parameters.AddRange(query.QueryPartsParameters().ToArray());

            return command;
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            var commands = new List<IDbCommand>();

            var command = new SqlCommand(string.Join(";\r\n", queries.Select(x => x.ToString())), (SqlConnection)GetConnection());
            command.Parameters.AddRange(queries.SelectMany(x => x.QueryPartsParameters()).ToArray());
            commands.Add(command);

            return commands;
        }
    }
}
