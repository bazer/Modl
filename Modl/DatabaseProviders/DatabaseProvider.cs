using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;

namespace Modl.DatabaseProviders
{
    public enum DatabaseType
    {
        SqlServer,
        SqlCE,
        MySQL
    }

    public abstract class DatabaseProvider
    {
        public DatabaseType Type;
        public string Name;
        public string ConnectionString;
        public string[] ProviderNames;
        protected IDbConnection activeConnection;

        protected DatabaseProvider(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        public abstract IDbConnection GetConnection();
        public abstract IDbCommand ToDbCommand(IQuery query);
        public abstract List<IDbCommand> ToDbCommands(List<IQuery> queries);
        public abstract IQuery GetLastIdQuery();

        public static DatabaseProvider GetNewDatabaseProvider(string databaseName, string connectionString, DatabaseType providerType)
        {
            string providerName = null;

            if (SqlServerProvider.Type == providerType)
                providerName = SqlServerProvider.ProviderNames[0];
            else if (SqlCeProvider.Type == providerType)
                providerName = SqlCeProvider.ProviderNames[0];
            else if (MySQLProvider.Type == providerType)
                providerName = MySQLProvider.ProviderNames[0];

            return GetNewDatabaseProvider(new ConnectionStringSettings(databaseName, connectionString, providerName));
        }

        public static DatabaseProvider GetNewDatabaseProvider(ConnectionStringSettings connectionConfig)
        {
            DatabaseProvider provider = SqlServerProvider.GetNewOnMatch(connectionConfig);
            provider = provider ?? SqlCeProvider.GetNewOnMatch(connectionConfig);
            provider = provider ?? MySQLProvider.GetNewOnMatch(connectionConfig);

            if (provider == null)
                throw new Exception(string.Format("Found no DatabaseProvider matching \"{0}\"", connectionConfig.ProviderName));

            return provider;
        }

        public static List<IDbCommand> GetDbCommands(List<IQuery> queries)
        {
            return queries.GroupBy(x => x.DatabaseProvider).SelectMany(x => x.Key.ToDbCommands(x.ToList())).ToList();
        }
    }
}
