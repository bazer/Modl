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

    public abstract class Database
    {
        public readonly DatabaseType Type;
        public readonly string Name;
        public readonly string ConnectionString;
        public readonly string Provider;

        protected string[] ProviderNames { get; set; }
        protected IDbConnection activeConnection;

        protected Database(string name, string connectionString, string provider)
        {
            Name = name;
            ConnectionString = connectionString;
            Provider = provider;
        }

        internal abstract IDbConnection GetConnection();
        internal abstract IDbCommand ToDbCommand(IQuery query);
        internal abstract List<IDbCommand> ToDbCommands(List<IQuery> queries);
        internal abstract IQuery GetLastIdQuery();

        internal static Database GetNewDatabaseProvider(string databaseName, string connectionString, DatabaseType providerType)
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

        internal static Database GetNewDatabaseProvider(ConnectionStringSettings connectionConfig)
        {
            Database provider = SqlServerProvider.GetNewOnMatch(connectionConfig);
            provider = provider ?? SqlCeProvider.GetNewOnMatch(connectionConfig);
            provider = provider ?? MySQLProvider.GetNewOnMatch(connectionConfig);

            if (provider == null)
                throw new Exception(string.Format("Found no DatabaseProvider matching \"{0}\"", connectionConfig.ProviderName));

            return provider;
        }

        public static Database Default
        {
            get
            {
                return Config.DefaultDatabase;
            }
            set
            {
                Config.DefaultDatabase = value;
            }
        }

        internal static Database AddFromConnectionString(ConnectionStringSettings connectionConfig)
        {
            return Config.AddDatabase(Database.GetNewDatabaseProvider(connectionConfig));
        }

        public static Database Add(Database database)
        {
            return Config.AddDatabase(database);
        }

        public static Database Add(string databaseName)
        {
            return AddFromConnectionString(ConfigurationManager.ConnectionStrings[databaseName]);
        }

        public static Database Add(string databaseName, string connectionString, string providerName)
        {
            return Config.AddDatabase(Database.GetNewDatabaseProvider(new ConnectionStringSettings(databaseName, connectionString, providerName)));
        }

        public static Database Add(string databaseName, string connectionString, DatabaseType providerType)
        {
            return Config.AddDatabase(Database.GetNewDatabaseProvider(databaseName, connectionString, providerType));
        }

        public static Database Get(string databaseName)
        {
            return Config.GetDatabase(databaseName);
        }

        public static List<Database> GetAll()
        {
            return Config.GetAllDatabases();
        }

        public static void Remove(string databaseName)
        {
            Config.RemoveDatabase(databaseName);
        }

        public static void RemoveAll()
        {
            Config.RemoveAllDatabases();
        }

        internal static List<IDbCommand> GetDbCommands(List<IQuery> queries)
        {
            return queries.GroupBy(x => x.DatabaseProvider).SelectMany(x => x.Key.ToDbCommands(x.ToList())).ToList();
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
