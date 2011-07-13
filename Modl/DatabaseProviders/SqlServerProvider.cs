using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

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

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            return new SqlCommand(query.ToString(), (SqlConnection)GetConnection());
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            //var connection = GetConnection();
            var commands = new List<IDbCommand>();

            commands.Add(new SqlCommand(string.Join(";\r\n", queries.Select(x => x.ToString())), (SqlConnection)GetConnection()));

            return commands;
        }
    }
}
