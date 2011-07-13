using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Modl.DatabaseProviders;

namespace Modl
{
    public enum DatabaseType
    {
        SqlServer,
        SqlCE,
        MySQL
    }

    public abstract class DatabaseProvider
    {
        public DatabaseType Type { get; protected set; }
        public string Name { get; protected set; }
        public string ConnectionString { get; protected set; }
        public string[] ProviderNames { get; protected set; }
        protected IDbConnection activeConnection;

        protected DatabaseProvider(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        internal abstract IDbConnection GetConnection();
        internal abstract IDbCommand ToDbCommand(IQuery query);
        internal abstract List<IDbCommand> ToDbCommands(List<IQuery> queries);
        internal abstract IQuery GetLastIdQuery();

        internal static DatabaseProvider GetNewDatabaseProvider(string databaseName, string connectionString, DatabaseType providerType)
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

        internal static DatabaseProvider GetNewDatabaseProvider(ConnectionStringSettings connectionConfig)
        {
            DatabaseProvider provider = SqlServerProvider.GetNewOnMatch(connectionConfig);
            provider = provider ?? SqlCeProvider.GetNewOnMatch(connectionConfig);
            provider = provider ?? MySQLProvider.GetNewOnMatch(connectionConfig);

            if (provider == null)
                throw new Exception(string.Format("Found no DatabaseProvider matching \"{0}\"", connectionConfig.ProviderName));

            return provider;
        }

        internal static List<IDbCommand> GetDbCommands(List<IQuery> queries)
        {
            return queries.GroupBy(x => x.DatabaseProvider).SelectMany(x => x.Key.ToDbCommands(x.ToList())).ToList();
        }

        public static DatabaseProvider GetDatabaseProvider(string databaseName)
        {
            return Config.GetDatabaseProvider(databaseName);
        }

        public T New<T>() where T : Modl<T>, new()
        {
            return Modl<T>.New(this);
        }

        public T Get<T>(int id, bool throwExceptionOnNotFound = true) where T : Modl<T>, new()
        {
            return Modl<T>.Get(id, this, throwExceptionOnNotFound);
        }

        public bool Exists<T>(int id) where T : Modl<T>, new()
        {
            return Modl<T>.Exists(id, this);
        }
    }
}
