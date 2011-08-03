using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Modl.Query;
using System.Linq.Expressions;

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
            return new Literal(Name, "SELECT last_insert_id()");
        }

        internal override string GetCommandString(string field, Relation relation, string key)
        {
            return string.Format("{0} {1} ?{2}", field, relation.ToSql(), key);
        }

        internal override IDbDataParameter GetCommandParameter(string key, object value)
        {
            return new MySqlParameter("?" + key, value);
        }

        internal override IDbCommand ToDbCommand(IQuery query)
        {
            var command = new MySqlCommand(query.ToString(), (MySqlConnection)GetConnection());
            command.Parameters.AddRange(query.QueryPartsParameters().ToArray());
            //foreach (var param in query.QueryPartsParameters())
            //    command.Parameters.Add(new MySqlParameter(param.Item1, param.Item2));

            return command;

            //return new MySqlCommand(query.ToString(), (MySqlConnection)GetConnection());
        }

        internal override List<IDbCommand> ToDbCommands(List<IQuery> queries)
        {
            //var connection = GetConnection();
            var commands = new List<IDbCommand>();

            //commands.Add(new MySqlCommand(string.Join(";\r\n", queries.Select(x => x.ToString())), (MySqlConnection)GetConnection()));

            var command = new MySqlCommand(string.Join(";\r\n", queries.Select(x => x.ToString())), (MySqlConnection)GetConnection());
            command.Parameters.AddRange(queries.SelectMany(x => x.QueryPartsParameters()).ToArray());
            //foreach (var param in queries.SelectMany(x => x.QueryPartsParameters()))
            //    command.Parameters.Add(new MySqlParameter(param.Item1, param.Item2));

            commands.Add(command);

            return commands;
        }

        //internal override object Execute(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
