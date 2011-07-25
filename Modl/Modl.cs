using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Modl.DataAccess;
using Modl.Exceptions;
using Modl.Fields;
using Modl.Query;
using Modl.Linq;
using System.Linq.Expressions;


namespace Modl
{
    public interface IModl
    {
        int Id { get; }
    }

    public abstract class ModlBase : IModl
    {
        public abstract int Id { get; }
        public static string TableName;
    }

    [ModelBinder(typeof(ModlBinder))]
    [DebuggerDisplay("{typeof(C).Name, nq}: {Id}")]
    public abstract class Modl<C> : ModlBase, System.IEquatable<C>
        where C : Modl<C>, new()
    {
        private bool isNew = true;
        public bool IsNew { get { return isNew; } }

        private bool isDeleted = false;
        public bool IsDeleted { get { return isDeleted; } }

        public bool IsDirty { get { return Store.IsDirty; } }
        public override int Id { get { return Store.Id; } }

        internal static string IdName = "Id";
        public string Table { get { return TableName; } }

        private Database instanceDbProvider = null;
        private static Database staticDbProvider = null;

        //public string DatabaseName { get { return Database.Name; } }

        public Database Database
        {
            get
            {
                if (instanceDbProvider == null)
                    instanceDbProvider = DefaultDatabase;

                return instanceDbProvider;
            }
        }

        /// <summary>
        /// The default database of this Modl entity. 
        /// This is the same as Config.DefaultDatabase unless a value is specified.
        /// Set to null to clear back to the value of Config.DefaultDatabase.
        /// </summary>
        public static Database DefaultDatabase
        {
            get
            {
                if (staticDbProvider == null)
                    return Config.DefaultDatabase;

                return staticDbProvider;
            }
            set
            {
                staticDbProvider = value;
            }
        }

        //public static void SetDefaultDatabase(string databaseName)
        //{
        //    DefaultDatabase = Config.GetDatabase(databaseName);
        //}

        //public static void ClearDefaultDatabase()
        //{
        //    staticDbProvider = null;
        //}

        //internal static dynamic Constants;
        protected dynamic Fields;
        protected dynamic F;
        private Dictionary<string, object> Lazy = new Dictionary<string, object>();
        internal Store<C> Store;

        static Modl()
        {
            typeof(C).TypeInitializer.Invoke(null, null);

            Statics<C>.IdName = IdName;
            Statics<C>.Initialize(new C());
        }

        //public static List<C> AllCached
        //{
        //    get
        //    {
        //        if (Constants.All == null)
        //            Constants.All = GetAll();

        //        return Constants.All;
        //    }
        //}

        public Modl()
        {
            Store = new Store<C>();

            Fields = Store.DynamicFields;
            F = Store.DynamicFields;
            //Lazy = new DynamicFields(store);

            Statics<C>.FillFields(this);
        }

        public static C New()
        {
            var c = new C();
            return c;
        }

        public static C New(string databaseName)
        {
            var c = new C();
            c.instanceDbProvider = Config.GetDatabase(databaseName);

            return c;
        }

        public static C New(Database database)
        {
            var c = new C();
            c.instanceDbProvider = database;

            return c;
        }

        public static bool Exists(int id, Database database = null)
        {
            return Get(id, database, false) != null;
        }

        public static bool Exists(Expression<Func<C, bool>> query, Database database = null)
        {
            return GetWhere(query, database, false) != null;
        }

        public static C Get(int id, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return Get(new Select<C>(database ?? DefaultDatabase).Where(IdName).EqualTo(id), throwExceptionOnNotFound);
        }

        private static C Get(Select<C> query, bool throwExceptionOnNotFound = true)
        {
            return Get(DbAccess.ExecuteReader(query), query.DatabaseProvider, throwExceptionOnNotFound, true);
        }

        protected static C Get(DbDataReader reader, Database database, bool throwExceptionOnNotFound = true, bool singleRow = true)
        {
            var c = Modl<C>.New(database); //new C();

            if (c.Store.Load(reader, throwExceptionOnNotFound, singleRow))
            {
                c.isNew = false;
                return c;
            }
            else
                return null;
        }

        //public static C GetCached(int id, bool throwExceptionOnNotFound = true)
        //{
        //    if (throwExceptionOnNotFound)
        //        return AllCached.Single(x => x.Id == id);
        //    else
        //        return AllCached.SingleOrDefault(x => x.Id == id);
        //}

        internal static IEnumerable<C> GetList(Select<C> query)
        {
            //var list = new List<C>();

            using (DbDataReader reader = DbAccess.ExecuteReader(query))
            {
                while (!reader.IsClosed)
                {
                    C c = Get(reader, query.DatabaseProvider, singleRow: false);

                    if (c != null)
                        yield return c;
                        //list.Add(c);
                }
            }

            //return list;
        }

        public static C GetWhere(Expression<Func<C, bool>> query, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return Get(new Select<C>(database ?? DefaultDatabase, query), throwExceptionOnNotFound);
        }

        //public static C GetWhere<T>(string field, T value, Database database = null, bool throwExceptionOnNotFound = true)
        //{
        //    return GetWhere(database, throwExceptionOnNotFound, Tuple.Create(field, value));
        //}

        //public static C GetWhere<T>(params Tuple<string, T>[] fields)
        //{
        //    return GetWhere(null, true, fields);
        //}

        //public static C GetWhere<T>(Database database = null, bool throwExceptionOnNotFound = true, params Tuple<string, T>[] fields)
        //{
        //    var select = new Select<C>(database ?? DefaultDatabase);

        //    foreach (var field in fields)
        //        select.Where(field.Item1).EqualTo(field.Item2.ToString());

        //    return Get(select, throwExceptionOnNotFound);
        //}

        public static IEnumerable<C> GetAll(Database database = null)
        {
            return GetList(new Select<C>(database ?? DefaultDatabase));
        }

        public static IEnumerable<C> GetAllWhere(Expression<Func<C, bool>> query, Database database = null)
        {
            return GetList(new Select<C>(database ?? DefaultDatabase, query));
        }

        //public static List<C> GetAllWhere<T>(string field, T value, Database database = null)
        //{
        //    return GetAllWhere(database, Tuple.Create(field, value));
        //}

        //public static List<C> GetAllWhere<T>(params Tuple<string, T>[] fields)
        //{
        //    return GetAllWhere(null, fields);
        //}

        //public static List<C> GetAllWhere<T>(Database database = null, params Tuple<string, T>[] fields)
        //{
        //    var select = new Select<C>(database ?? DefaultDatabase);

        //    foreach (var field in fields)
        //        select.Where(field.Item1).EqualTo(field.Item2.ToString());

        //    return GetList(select);
        //}

        //protected void SetValue<T>(string name, T value)
        //{
        //    Fields.SetValue(name, value);
        //}

        //protected T GetValue<T>(string name)
        //{
        //    return Fields.GetValue<T>(name);
        //}

        protected delegate T FetchDelegate<T>();
        protected T GetLazy<T>(string name, FetchDelegate<T> fetchCode)
        {
            if (!Lazy.ContainsKey(name) || Lazy[name] == null)
                Lazy[name] = fetchCode();

            return (T)Lazy[name];
        }

        protected void SetLazy<T>(string name, T value)
        {
            Lazy[name] = value;
        }

        protected delegate T FetchIdDelegate<T>(int id);
        protected T GetFk<T>(string name, FetchIdDelegate<T> fetchCode, bool execOnZeroVal = false)
        {
            if (Fields.Dictionary[name] is int)
            {
                if (!execOnZeroVal && Fields.Dictionary[name] == 0)
                    return default(T);
                else
                    Fields.Dictionary[name] = fetchCode(Fields.Dictionary[name]);
            }
            else if (execOnZeroVal && Fields.Dictionary[name] == null)
                Fields.Dictionary[name] = fetchCode(Helper.ConvertTo<int>(Fields.Dictionary[name]));

            return Fields.Dictionary[name];
        }

        protected void SetFk<T>(string name, T value)
        {
            Fields.Dictionary[name] = value;
        }



        public virtual void Save(Modl.DataAccess.DbTransaction dbTransaction = null)
        {
            Change<C> statement = BaseGetSaveStatement();

            Store.BaseAddSaveFields(statement);

            if (dbTransaction != null)
                BaseTransactionSave(statement, dbTransaction);
            else
                BaseSave(statement);

            Store.ResetFields();
            //LogSave();
        }

        private Change<C> BaseGetSaveStatement()
        {
            Change<C> statement;

            if (isNew)
            {
                statement = new Insert<C>(Database);
            }
            else
            {
                statement = new Update<C>(Database);
                ((Update<C>)statement).Where(IdName).EqualTo(Id);
            }

            return statement;
        }



        private void BaseSave(Change<C> statement)
        {
            if (isDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", TableName, Id));

            if (statement is Insert<C>)
                Store.Id = DbAccess.ExecuteScalar<int>(statement, Database.GetLastIdQuery());
            else
                DbAccess.ExecuteNonQuery(statement);

            isNew = false;
        }

        private void BaseTransactionSave(Change<C> statement, Modl.DataAccess.DbTransaction trans)
        {
            if (isDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", TableName, Id));

            //if (statement is Insert<C>)
            //    id = trans.ExecuteScalar<int>(statement, new Literal("select SCOPE_IDENTITY()"));
            //else
            //    trans.ExecuteNonQuery(statement);

            isNew = false;
        }

        public virtual void Delete()
        {
            //LogDelete();

            Delete<C> statement = new Delete<C>(Database);
            statement.Where(IdName).EqualTo(Id);

            DbAccess.ExecuteNonQuery(statement);

            isDeleted = true;
        }

        //protected void LogSave()
        //{
        //    //if (isNew)
        //    //    Log.Create(null, this);
        //    //else
        //    //    Log.Edit(null, this);
        //}

        //protected void LogDelete()
        //{
        //    //Log.Delete(null, this);
        //}

        public override string ToString()
        {
            return Id.ToString();
        }

        #region IEquatable<C> Members

        public bool Equals(C other)
        {
            return this.Id == other.Id;
        }

        #endregion


        //protected static Tuple<string, T> Q<T>(string field, T value)
        //{
        //    return Tuple.Create<string, T>(field, value);
        //}

        

        //public Select<C> Select()
        //{
        //    return new Select<C>(Database);
        //}

        //public Where<C, K> Where<K>(string key) //where K : Query<C, K>
        //{
        //    return new Where<C, K>(key);
        //}

        //public Where<C, Select<C>> Where(string key)
        //{
        //    return new Where<C, Select<C>>(key);
        //}

        //public Where<C, Update<C>> Where(string key)
        //{
        //    return new Where<C, Update<C>>(key);
        //}

        //public Where<C, Select<C>> Where(string key) 
        //{
        //    return new Where<C, Select<C>>(key);
        //}

        public static IQueryable<C> Query(Database database = null)
        {
            return new LinqQuery<C>(database ?? DefaultDatabase);
        }

        internal static string GetFieldName(string propertyName)
        {
            if (propertyName == "Id")
                return Statics<C>.IdName;
            else
                return Statics<C>.GetFieldName(propertyName);
        }

        //internal static string GetFieldName(Expression<Func<C, string>> field)
        //{
        //    return Statics<C>.GetFieldName((string)LinqHelper.GetValue<C>(field));
        //}
        
    }
}
