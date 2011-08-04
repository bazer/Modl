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
            var sql = query.ToSql();
            var command = new SqlCeCommand(sql.Item1, (SqlCeConnection)GetConnection());
            command.Parameters.AddRange(sql.Item2.ToArray());

            return command;

            //return new SqlCeCommand(query.ToString(), (SqlCeConnection)GetConnection());
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            var connection = GetConnection();
            var commands = new List<IDbCommand>();

            foreach (var query in queries)
            {
                var sql = query.ToSql();
                var command = new SqlCeCommand(sql.Item1, (SqlCeConnection)connection);
                command.Parameters.AddRange(sql.Item2.ToArray());
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
