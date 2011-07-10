using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Modl.DatabaseProviders
{
    public class SqlServerProvider : DatabaseProvider
    {
        public static new DatabaseType Type = DatabaseType.SqlServer;
        public static new string[] ProviderNames = new string[1] { "System.Data.SqlClient" };

        protected SqlServerProvider(string name, string connectionString) : base(name, connectionString) { }

        public override IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static SqlServerProvider GetNewOnMatch(ConnectionStringSettings connectionConfig)
        {
            if (ProviderNames.Contains(connectionConfig.ProviderName))
                return new SqlServerProvider(connectionConfig.Name, connectionConfig.ConnectionString);

            return null;
        }

        public override IQuery GetLastIdQuery()
        {
            return new Literal(Name, "SELECT SCOPE_IDENTITY()");
        }

        public override IDbCommand ToDbCommand(IQuery query)
        {
            return new SqlCommand(query.ToString(), (SqlConnection)GetConnection());
        }

        public override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            //var connection = GetConnection();
            var commands = new List<IDbCommand>();

            commands.Add(new SqlCommand(string.Join(";\r\n", queries.Select(x => x.ToString())), (SqlConnection)GetConnection()));

            return commands;
        }
    }
}
