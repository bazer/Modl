using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Configuration;
using Modl.DatabaseProviders;
using System.Data;

namespace Modl
{
    public class Config
    {
        //public static string ConnectionString { get; set; }
        public static Dictionary<string, DatabaseProvider> DatabaseProviders = new Dictionary<string, DatabaseProvider>();

        static Config()
        {
            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
                if (!string.IsNullOrWhiteSpace(connString.ConnectionString) && !string.IsNullOrWhiteSpace(connString.Name) && !string.IsNullOrWhiteSpace(connString.ProviderName))
                    AddDatabaseProvider(connString);

            //if (ConfigurationManager.ConnectionStrings.Count > 1)
            //    AddDatabaseProvider(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1]);
        }

        public static IDbConnection GetConnection(string databaseName)
        {
            
            return DatabaseProviders[databaseName].GetConnection();
        }

        public static void AddDatabaseProvider(ConnectionStringSettings connectionConfig)
        {
            DatabaseProviders[connectionConfig.Name] = DatabaseProvider.GetNewDatabaseProvider(connectionConfig);
        }

        public static void AddDatabaseProvider(string databaseName)
        {
            AddDatabaseProvider(ConfigurationManager.ConnectionStrings[databaseName]);
        }

        public static void AddDatabaseProvider(string databaseName, string connectionString, string providerName)
        {
            DatabaseProviders[databaseName] = DatabaseProvider.GetNewDatabaseProvider(new ConnectionStringSettings(databaseName, connectionString, providerName));
        }

        public static void AddDatabaseProvider(string databaseName, string connectionString, DatabaseType providerType)
        {
            DatabaseProviders[databaseName] = DatabaseProvider.GetNewDatabaseProvider(databaseName, connectionString, providerType);
        }
    }
}
