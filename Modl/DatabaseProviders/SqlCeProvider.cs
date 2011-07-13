using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlServerCe;

namespace Modl.DatabaseProviders
{
    public class SqlCeProvider : Database
    {
        public static new DatabaseType Type = DatabaseType.SqlCE;
        public static new string[] ProviderNames = new string[1] { "Microsoft.SQLSERVER.CE.OLEDB.4.0" };

        protected SqlCeProvider(string name, string connectionString) : base(name, connectionString) { }

        internal override IDbConnection GetConnection()
        {
            //if (activeConnection.State != ConnectionState.Closed)
                activeConnection = new SqlCeConnection(ConnectionString);

            return activeConnection;
        }

        public static SqlCeProvider GetNewOnMatch(ConnectionStringSettings connectionConfig)
        {
            if (ProviderNames.Contains(connectionConfig.ProviderName))
                return new SqlCeProvider(connectionConfig.Name, connectionConfig.ConnectionString);

            return null;
        }

        internal override IQuery GetLastIdQuery()
        {
            return new Literal(Name, "SELECT @@IDENTITY");
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            return new SqlCeCommand(query.ToString(), (SqlCeConnection)GetConnection());
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            var connection = GetConnection();

            return queries.Select(x => new SqlCeCommand(x.ToString(), (SqlCeConnection)connection)).ToList<IDbCommand>();

            //List<IDbCommand> commands = new List<IDbCommand>();

            //foreach (var query in queries)
            //    commands.Add(new SqlCeCommand(query.ToString()));

            //return commands;
        }
    }
}
