/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

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
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Modl.DataAccess;
using Modl.Fields;
using Modl.Linq;
using Modl.Query;
using Modl.Cache;


namespace Modl
{
    public interface IModl
    {
        int Id { get; }
    }

    public interface IModl<IdType>
    {
        IdType Id { get; }
    }

    //public interface IModl<M>
    //    //where M : IModl<M>, new()
    //{
    //    int Id { get; }
    //}

    //public abstract class Modl<M, IdType> : Modl<M>
    //    where M : Modl<M>, new()
    //{
    //    public new IdType Id { get { return (IdType)Store.Id; } }

    //    public Modl() 
    //    {
    //        Store.IdType = typeof(IdType);
    //    }
    //}

    public abstract class Modl<M, IdType> : Modl<M>, IModl<IdType>
        where M : Modl<M, IdType>, new()
    {
        public new IdType Id { get { return (IdType)Store.Id; } }

        public Modl()
        {
            Store.IdType = typeof(IdType);
        }

        public Modl(IdType id)
        {
            Store.IdType = typeof(IdType);
            SetId(id);
            //Store.Id = id;
            //AutomaticId = false;
        }

        public static M New(IdType id)
        {
            var m = new M();
            m.SetId(id);

            return m;
        }

        public static M New(IdType id, Database database)
        {
            var m = New(database);
            m.SetId(id);

            return m;
        }

        public static M New(IdType id, string databaseName)
        {
            var m = New(databaseName);
            m.SetId(id);

            return m;
        }

        //internal static new M New(int id) { throw new NotImplementedException(); }
        //internal static new M New(int id, Database database) { throw new NotImplementedException(); }
        //internal static new M New(int id, string databaseName) { throw new NotImplementedException(); }

        public static bool Exists(IdType id, Database database = null)
        {
            return Get(id, database, false) != null;
        }

        public static M Get(IdType id, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return Modl<M>.Get<IdType>(id, database, throwExceptionOnNotFound);
        }

        public static new IEnumerable<M> GetAll(Database database = null)
        {
            //return new Select<M>(database ?? DefaultDatabase).GetList<IdType>();
            return GetList<IdType>(new Select<M>(database ?? DefaultDatabase));
        }

        public static new IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            //return new Select<M>(database ?? DefaultDatabase, query).GetList<IdType>();
            return GetList<IdType>(new Select<M>(database ?? DefaultDatabase, query));
        }

        public override void Save()
        {
            Save<IdType>();
        }

        public override void Delete()
        {
            Delete<IdType>(Id, Database);
            isDeleted = true;
        }

        public static void Delete(IdType id, Database database = null)
        {
            Delete<IdType>(id, database);
        }

        public static new void DeleteAll(Database database = null)
        {
            DeleteAll<IdType>(database);
        }
    }

    //[ModelBinder(typeof(ModlBinder))]
    [DebuggerDisplay("{typeof(M).Name, nq}: {Id}")]
    public abstract class Modl<M> : System.IEquatable<M>, IModl , IModl<int>
        where M : Modl<M>, new()
    {
        internal bool isNew = true;
        public bool IsNew { get { return isNew; } }

        internal bool isDeleted = false;
        public bool IsDeleted { get { return isDeleted; } }

        public bool IsDirty { get { Statics<M>.ReadFromEmptyProperties(this); return Store.IsDirty; } }
        public int Id { get { return (int)Store.Id; } }
        internal bool AutomaticId = true;

        internal static string IdName { get { return Statics<M>.IdName; } }
        internal static string Table { get { return Statics<M>.TableName; } }

        private Database instanceDbProvider = null;
        private static Database staticDbProvider = null;

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

        //internal static dynamic Constants;
        protected dynamic Fields;
        protected dynamic F;
        private Dictionary<string, object> Lazy = new Dictionary<string, object>();
        internal Store<M> Store;
        //internal virtual Store<M, int> Store;

        static Modl()
        {
            Statics<M>.Initialize(new M());
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
            Initialize(typeof(int));
        }

        public Modl(int id)
        {
            Initialize(typeof(int));
            SetId(id);
        }

        internal void SetId(object id)
        {
            Store.Id = id;
            AutomaticId = false;
        }

        private void Initialize(Type type)
        {
            Store = new Store<M>(typeof(int));
            Fields = Store.DynamicFields;
            F = Store.DynamicFields;

            Statics<M>.FillFields(this);
        }

        public static M New()
        {
            return new M();
        }

        public static M New(int id)
        {
            var m = new M();
            m.SetId(id);

            return m;
        }

        public static M New(int id, Database database)
        {
            var m = New(database);
            m.SetId(id);

            return m;
        }

        public static M New(int id, string databaseName)
        {
            var m = New(databaseName);
            m.SetId(id);

            return m;
        }

        public static M New(Database database)
        {
            var c = new M();
            c.instanceDbProvider = database;

            return c;
        }

        public static M New(string databaseName)
        {
            var c = new M();
            c.instanceDbProvider = Config.GetDatabase(databaseName);

            return c;
        }



        public static bool Exists(int id, Database database = null)
        {
            return Get(id, database, false) != null;
        }

        public static bool Exists(Expression<Func<M, bool>> query, Database database = null)
        {
            return GetWhere(query, database, false) != null;
        }

        public static M Get(int id, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return Get<int>(id, database, throwExceptionOnNotFound);
            //return new Select<M>(database ?? DefaultDatabase).Where(IdName).EqualTo(id).Get(throwExceptionOnNotFound);
            //return Get(new Select<M>(database ?? DefaultDatabase).Where(IdName).EqualTo(id), throwExceptionOnNotFound);
        }

        internal static M Get<IdType>(IdType id, Database database, bool throwExceptionOnNotFound)
        {
            return StaticCache<M, IdType>.Get(id, database ?? DefaultDatabase, throwExceptionOnNotFound);

            //Database db = database ?? DefaultDatabase;

            //if (StaticCache<M, IdType>.Contains(id, db))
            //    return StaticCache<M, IdType>.Get(id, db);
            //else
            //{
            //    M instance = new Select<M>(db).Where(IdName).EqualTo(id).Get(throwExceptionOnNotFound);

            //}
        }

        internal static M Get(DbDataReader reader, Database database, bool throwExceptionOnNotFound = true, bool singleRow = true)
        {
            var m = Modl<M>.New(database);

            if (m.Store.Load(reader, throwExceptionOnNotFound, singleRow))
            {
                Statics<M>.WriteToEmptyProperties(m);
                m.isNew = false;

                return m;
            }
            else
                return null;
        }

        public static M GetWhere(Expression<Func<M, bool>> query, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return new Select<M>(database ?? DefaultDatabase, query).Get(throwExceptionOnNotFound);
            //return Get(new Select<M>(database ?? DefaultDatabase, query), throwExceptionOnNotFound);
        }

        public static IEnumerable<M> GetAll(Database database = null)
        {
            return GetList<int>(new Select<M>(database ?? DefaultDatabase));
            //return GetList(new Select<M>(database ?? DefaultDatabase));
        }

        public static IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            return GetList<int>(new Select<M>(database ?? DefaultDatabase, query));
            //return GetList(new Select<M>(database ?? DefaultDatabase, query));
        }

        internal static IEnumerable<M> GetList<IdType>(Select<M> query)
        {
            using (DbDataReader reader = query.Execute())
            {
                while (!reader.IsClosed)
                {
                    var c = Get(reader, query.DatabaseProvider, singleRow: false);

                    if (c != null)
                        yield return c;
                }
            }
        }

        protected void SetValue<T>(string name, T value)
        {
            Store.SetValue(name, value);
        }

        protected T GetValue<T>(string name)
        {
            return Store.GetValue<T>(name);
        }

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



        //public virtual void Save(Modl.DataAccess.DbTransaction dbTransaction = null)
        //{
        //    Change<M> statement = BaseGetSaveStatement();
        //    Statics<M>.ReadFromEmptyProperties(this);
        //    Store.BaseAddSaveFields(statement);

        //    if (dbTransaction != null)
        //        BaseTransactionSave(statement, dbTransaction);
        //    else
        //        BaseSave(statement);

        //    Store.ResetFields();
        //    //LogSave();
        //}

        //private Change<M> BaseGetSaveStatement()
        //{
        //    Change<M> statement;

        //    if (IsNew)
        //    {
        //        statement = new Insert<M>(Database);
        //    }
        //    else
        //    {
        //        statement = new Update<M>(Database);
        //        ((Update<M>)statement).Where(IdName).EqualTo(Id);
        //    }

        //    return statement;
        //}


        public virtual void Save()
        {
            Save<int>();
        }

        internal void Save<IdType>()
        {
            if (isDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", Table, Store.Id));

            Change<M> query;

            if (IsNew)
                query = new Insert<M>(Database);
            else
                query = new Update<M>(Database).Where(IdName).EqualTo(Store.Id);

            Statics<M>.ReadFromEmptyProperties(this);
            Store.BaseAddSaveFields(query);

            if (query is Insert<M>)
            {
                if (!AutomaticId)
                {
                    query.With(IdName, Store.Id);
                    AsyncDbAccess.ExecuteNonQuery(query);
                }
                else
                    Store.Id = DbAccess.ExecuteScalar<IdType>(query, Database.GetLastIdQuery());

                StaticCache<M, IdType>.Add((IdType)Store.Id, (M)this, Database);
            }
            else
                AsyncDbAccess.ExecuteNonQuery(query);

            isNew = false;
            Store.ResetFields();
        }

        //private void BaseTransactionSave(Change<M> statement, Modl.DataAccess.DbTransaction trans)
        //{
        //    if (isDeleted)
        //        throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", Table, Id));

        //    //if (statement is Insert<C>)
        //    //    id = trans.ExecuteScalar<int>(statement, new Literal("select SCOPE_IDENTITY()"));
        //    //else
        //    //    trans.ExecuteNonQuery(statement);

        //    isNew = false;
        //}

        public virtual void Delete()
        {
            Delete(Id, Database);

            //LogDelete();

            //Delete<C> statement = new Delete<C>(Database);
            //statement.Where(IdName).EqualTo(Id);

            //DbAccess.ExecuteNonQuery(statement);

            isDeleted = true;
        }

        public static void Delete(int id, Database database = null)
        {
            Delete<int>(id, database);
        }

        internal static void Delete<IdType>(IdType id, Database database = null)
        {
            Delete<M> statement = new Delete<M>(database ?? DefaultDatabase);
            statement.Where(IdName).EqualTo(id);

            AsyncDbAccess.ExecuteNonQuery(statement);

            StaticCache<M, IdType>.Delete(id, database ?? DefaultDatabase);
        }

        public static void DeleteAll(Database database = null)
        {
            DeleteAll<int>(database);
        }

        public static void DeleteAll<IdType>(Database database = null)
        {
            Delete<M> statement = new Delete<M>(database ?? DefaultDatabase);

            AsyncDbAccess.ExecuteNonQuery(statement);

            StaticCache<M, IdType>.DeleteAll(database ?? DefaultDatabase);
        }

        public static void DeleteAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            DeleteAllWhere<int>(query, database);
        }

        public static void DeleteAllWhere<IdType>(Expression<Func<M, bool>> query, Database database = null)
        {
            foreach (var modl in GetAllWhere(query, database).ToList().Cast<IModl<IdType>>())
                StaticCache<M, IdType>.Delete(modl.Id, database ?? DefaultDatabase);

            Delete<M> statement = new Delete<M>(database ?? DefaultDatabase, query);

            AsyncDbAccess.ExecuteNonQuery(statement);
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

        public bool Equals(M other)
        {
            return this.Id == other.Id;
        }

        #endregion


        public static IQueryable<M> Query(Database database = null)
        {
            return new LinqQuery<M>(database ?? DefaultDatabase);
        }

        internal static string GetFieldName(string propertyName)
        {
            if (propertyName == "Id")
                return IdName;
            else
                return Statics<M>.GetFieldName(propertyName);
        }

        //internal static string GetFieldName(Expression<Func<C, string>> field)
        //{
        //    return Statics<C>.GetFieldName((string)LinqHelper.GetValue<C>(field));
        //}
    }
}
