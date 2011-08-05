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


namespace Modl
{
    public interface IModl
    {
        int Id { get; }
    }

    public abstract class ModlBase : IModl
    {
        public abstract int Id { get; }
    }

    //[ModelBinder(typeof(ModlBinder))]
    [DebuggerDisplay("{typeof(C).Name, nq}: {Id}")]
    public abstract class Modl<M> : ModlBase, System.IEquatable<M>
        where M : Modl<M>, new()
    {
        private bool isNew = true;
        public bool IsNew { get { return isNew; } }

        private bool isDeleted = false;
        public bool IsDeleted { get { return isDeleted; } }

        public bool IsDirty { get { return Store.IsDirty; } }
        public override int Id { get { return Store.Id; } }

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
            Store = new Store<M>();

            Fields = Store.DynamicFields;
            F = Store.DynamicFields;

            Statics<M>.FillFields(this);
        }

        public static M New()
        {
            return new M();
        }

        public static M New(string databaseName)
        {
            var c = new M();
            c.instanceDbProvider = Config.GetDatabase(databaseName);

            return c;
        }

        public static M New(Database database)
        {
            var c = new M();
            c.instanceDbProvider = database;

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
            return Get(new Select<M>(database ?? DefaultDatabase).Where(IdName).EqualTo(id), throwExceptionOnNotFound);
        }

        internal static M Get(Select<M> query, bool throwExceptionOnNotFound = true)
        {
            return Get(DbAccess.ExecuteReader(query), query.DatabaseProvider, throwExceptionOnNotFound, true);
        }

        protected static M Get(DbDataReader reader, Database database, bool throwExceptionOnNotFound = true, bool singleRow = true)
        {
            var m = Modl<M>.New(database);

            if (m.Store.Load(reader, throwExceptionOnNotFound, singleRow))
            {
                m.isNew = false;
                return m;
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

        internal static IEnumerable<M> GetList(Select<M> query)
        {
            using (DbDataReader reader = DbAccess.ExecuteReader(query))
            {
                while (!reader.IsClosed)
                {
                    var c = Get(reader, query.DatabaseProvider, singleRow: false);

                    if (c != null)
                        yield return c;
                }
            }
        }

        public static M GetWhere(Expression<Func<M, bool>> query, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return Get(new Select<M>(database ?? DefaultDatabase, query), throwExceptionOnNotFound);
        }

        public static IEnumerable<M> GetAll(Database database = null)
        {
            return GetList(new Select<M>(database ?? DefaultDatabase));
        }

        public static IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            return GetList(new Select<M>(database ?? DefaultDatabase, query));
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



        public virtual void Save(Modl.DataAccess.DbTransaction dbTransaction = null)
        {
            Change<M> statement = BaseGetSaveStatement();

            Store.BaseAddSaveFields(statement);

            if (dbTransaction != null)
                BaseTransactionSave(statement, dbTransaction);
            else
                BaseSave(statement);

            Store.ResetFields();
            //LogSave();
        }

        private Change<M> BaseGetSaveStatement()
        {
            Change<M> statement;

            if (isNew)
            {
                statement = new Insert<M>(Database);
            }
            else
            {
                statement = new Update<M>(Database);
                ((Update<M>)statement).Where(IdName).EqualTo(Id);
            }

            return statement;
        }



        private void BaseSave(Change<M> statement)
        {
            if (isDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", Table, Id));

            if (statement is Insert<M>)
                Store.Id = DbAccess.ExecuteScalar<int>(statement, Database.GetLastIdQuery());
            else
                DbAccess.ExecuteNonQuery(statement);

            isNew = false;
        }

        private void BaseTransactionSave(Change<M> statement, Modl.DataAccess.DbTransaction trans)
        {
            if (isDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", Table, Id));

            //if (statement is Insert<C>)
            //    id = trans.ExecuteScalar<int>(statement, new Literal("select SCOPE_IDENTITY()"));
            //else
            //    trans.ExecuteNonQuery(statement);

            isNew = false;
        }

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
            Delete<M> statement = new Delete<M>(database ?? DefaultDatabase);
            statement.Where(IdName).EqualTo(id);

            DbAccess.ExecuteNonQuery(statement);
        }

        public static void DeleteAll(Database database = null)
        {
            Delete<M> statement = new Delete<M>(database ?? DefaultDatabase);
            
            DbAccess.ExecuteNonQuery(statement);
        }

        public static void DeleteAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            Delete<M> statement = new Delete<M>(database ?? DefaultDatabase, query);

            DbAccess.ExecuteNonQuery(statement);
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
