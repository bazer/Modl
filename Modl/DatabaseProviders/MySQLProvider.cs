using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Modl.DatabaseProviders
{
    public class MySQLProvider : DatabaseProvider
    {
        public static new DatabaseType Type = DatabaseType.MySQL;
        public static new string[] ProviderNames = new string[1] { "MySql.Data.MySqlClient" };

        protected MySQLProvider(string name, string connectionString) : base(name, connectionString) { }

        internal override IDbConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static MySQLProvider GetNewOnMatch(ConnectionStringSettings connectionConfig)
        {
            if (ProviderNames.Contains(connectionConfig.ProviderName))
                return new MySQLProvider(connectionConfig.Name, connectionConfig.ConnectionString);

            return null;
        }

        internal override IQuery GetLastIdQuery()
        {
            return new Literal(Name, "SELECT last_insert_id()");
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            return new MySqlCommand(query.ToString(), (MySqlConnection)GetConnection());
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            //var connection = GetConnection();
            var commands = new List<IDbCommand>();

            commands.Add(new MySqlCommand(string.Join(";\r\n", queries.Select(x => x.ToString())), (MySqlConnection)GetConnection()));

            return commands;
        }
    }
}
