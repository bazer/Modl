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

        internal override string GetParameterValue(string key)
        {
            return string.Format("?{0}", key);
        }

        internal override string GetParameterComparison(string field, Relation relation, string key)
        {
            return string.Format("{0} {1} ?{2}", field, relation.ToSql(), key);
        }

        internal override IDataParameter GetParameter(string key, object value)
        {
            return new MySqlParameter("?" + key, value);
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            var sql = query.ToSql();
            var command = new MySqlCommand(sql.Item1, (MySqlConnection)GetConnection());
            command.Parameters.AddRange(sql.Item2.ToArray());

            return command;
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            var sql = queries.Select(x => x.ToSql()).ToList();
            var commands = new List<IDbCommand>();

            var command = new MySqlCommand(string.Join("; \r\n", sql.Select(x => x.Item1)), (MySqlConnection)GetConnection());
            command.Parameters.AddRange(sql.SelectMany(x => x.Item2).ToArray());
            commands.Add(command);

            return commands;
        }
    }
}
