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
        protected static Dictionary<string, Database> DatabaseProviders = new Dictionary<string, Database>();

        static Config()
        {
            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
                if (!string.IsNullOrWhiteSpace(connString.ConnectionString) && !string.IsNullOrWhiteSpace(connString.Name) && !string.IsNullOrWhiteSpace(connString.ProviderName))
                    Database.AddFromConnectionString(connString);

            //if (ConfigurationManager.ConnectionStrings.Count > 1)
            //    AddDatabaseProvider(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1]);
        }

        private static Database defaultDbProvider = null;
        internal static Database DefaultDatabase
        {
            get
            {
                if (defaultDbProvider == null)
                    defaultDbProvider = Config.DatabaseProviders.Last().Value;

                return defaultDbProvider;
            }
            set
            {
                defaultDbProvider = value;
            }
        }

        internal static Database AddDatabase(Database database)
        {
            DatabaseProviders[database.Name] = database;

            return database;
        }

        internal static Database GetDatabase(string databaseName)
        {
            return DatabaseProviders[databaseName];
        }

        internal static List<Database> GetAllDatabases()
        {
            return DatabaseProviders.Values.ToList();
        }

        internal static void RemoveDatabase(string databaseName)
        {
            DatabaseProviders.Remove(databaseName);
        }

        internal static void RemoveAllDatabases()
        {
            DatabaseProviders.Clear();
        }

        //public static IDbConnection GetConnection(string databaseName)
        //{
        //    return DatabaseProviders[databaseName].GetConnection();
        //}

        
    }
}
