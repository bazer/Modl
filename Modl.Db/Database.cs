﻿/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Modl.Db.DatabaseProviders;
using Modl.Db.Query;
using Modl.Db.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Modl.Db.DataAccess;
using Modl.Cache;

namespace Modl.Db
{
    public enum DatabaseType
    {
        SqlServer,
        SqlCE,
        MySQL
    }

    public abstract class Database : IDisposable
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
        internal abstract Sql GetParameter(Sql sql, string key, object value);
        internal abstract Sql GetParameterValue(Sql sql, string key);
        internal abstract Sql GetParameterComparison(Sql sql, string field, Relation relation, string key);
        

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

        //public M New<M, IdType>() where M : Modl<M, IdType>, new()
        //{
        //    return Modl<M, IdType>.New(this);
        //}

        //public M Get<M, IdType>(IdType id, bool throwExceptionOnNotFound = true) where M : Modl<M, IdType>, new()
        //{
        //    return Modl<M, IdType>.Get(id, this, throwExceptionOnNotFound);
        //}

        //public bool Exists<M, IdType>(IdType id) where M : Modl<M, IdType>, new()
        //{
        //    return Modl<M, IdType>.Exists(id, this);
        //}

        //public IQueryable<M> Query<M, IdType>() where M : Modl<M, IdType>, new()
        //{
        //    return Modl<M, IdType>.Query(this);
        //    //return new LinqQuery<M, IdType>(this);
        //}

        //public IQueryable<M> Query<M>() where M : Modl<M>, new()
        //{
        //    return Modl<M>.Query(this);
        //    //return new LinqQuery<M, IdType>(this);
        //}

        //IQueryable<T> IQueryProvider.CreateQuery<T>(System.Linq.Expressions.Expression expression)
        //{
        //    return new LinqQuery<T>(this, expression);
        //}

        //IQueryable IQueryProvider.CreateQuery(System.Linq.Expressions.Expression expression)
        //{
        //    throw new NotImplementedException();
        //}

        //T IQueryProvider.Execute<T>(System.Linq.Expressions.Expression expression)
        //{
        //    //var select = new Select<T>(this, expression);

        //    return (T)this.Execute(expression);
        //}

        //object IQueryProvider.Execute(System.Linq.Expressions.Expression expression)
        //{
        //    //return this.Execute(expression);
        //    throw new NotImplementedException();
        //}

        //object Execute(Expression expression)
        //{
        //    //Type elementType = TypeSystem.GetElementType(expression.Type);
            

        //    //var method = expression.Type.GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).Single(x => x.Name == "Get" && x.GetParameters().Count() == 2);
        //    //return method.Invoke(null, new object[] { Convert.ToInt32(value.AttemptedValue), true });

        //    //return Activator.CreateInstance(typeof(Modl<>).MakeGenericType(expression.Type), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { reader }, null);
        //}

        public static void DisposeAll()
        {
            AsyncDbAccess.DisposeAllWorkers();
            CacheConfig.Clear();
        }

        public void Dispose()
        {
            AsyncDbAccess.DisposeWorker(this);
        }
    }
}
