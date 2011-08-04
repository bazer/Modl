﻿using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Modl.Query;

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
            return new Literal(this, "SELECT SCOPE_IDENTITY()");
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
            return new SqlParameter("@" + key, value);
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            var sql = query.ToSql();
            var command = new SqlCommand(sql.Item1, (SqlConnection)GetConnection());
            command.Parameters.AddRange(sql.Item2.ToArray());

            return command;
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            var sql = queries.Select(x => x.ToSql()).ToList();
            var commands = new List<IDbCommand>();

            var command = new SqlCommand(string.Join("; \r\n", sql.Select(x => x.Item1)), (SqlConnection)GetConnection());
            command.Parameters.AddRange(sql.SelectMany(x => x.Item2).ToArray());
            commands.Add(command);

            return commands;
        }
    }
}
