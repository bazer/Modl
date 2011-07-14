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


namespace Modl
{
    public interface IModl
    {
        int Id { get; }
        //IModl Get(int id, bool throwExceptionOnNotFound = true);
        //string Table { get; }
    }

    public abstract class ModlBase : IModl
    {
        //internal int id = 0;
        public abstract int Id { get; }

        public static string TableName;

        //IModl Get(int id, bool throwExceptionOnNotFound = true);
    }

    [ModelBinder(typeof(ModlBinder))]
    [DebuggerDisplay("{typeof(C).Name, nq}: {Id}")] //: 
    public abstract class Modl<C> : ModlBase, System.IEquatable<C>
        where C : Modl<C>, new()
    {
        private bool isNew = true;
        public bool IsNew { get { return isNew; } }

        private bool isDeleted = false;
        public bool IsDeleted { get { return isDeleted; } }

        //private int id = 0;
        public override int Id { get { return store.Id; } }

        //public static string TableName;
        internal static string IdName = "Id";
        //protected static string NameField;
        public string Table { get { return TableName; } }

        public string DatabaseName
        {
            get
            {
                return Database.Name;
            }
        }
        
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

        public static Database DefaultDatabase 
        {
            get 
            {
                if (staticDbProvider == null)
                    return Config.DefaultDatabase;

                return staticDbProvider;
            }
        }

        public static void SetDefaultDatabase(string databaseName)
        {
            SetDefaultDatabase(Config.GetDatabase(databaseName));
        }

        public static void SetDefaultDatabase(Database database)
        {
            staticDbProvider = database;
        }

        public static void ClearDefaultDatabase()
        {
            staticDbProvider = null;
        }

        public static dynamic Constants;// = new DynamicFields();
        //public static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        protected dynamic Fields;
        protected dynamic F;
        protected dynamic Lazy;// = new DynamicFields();
        private Store<C> store;

        //public virtual string Name { get { return string.IsNullOrEmpty(NameField) ? "" : Fields.Dictionary[NameField].Trim(); } set { if (!string.IsNullOrEmpty(NameField)) Fields.Dictionary[NameField] = value; } }
        //public virtual Description

        static Modl()
        {
            typeof(C).TypeInitializer.Invoke(null, null);
        }

        public static List<C> AllCached
        {
            get
            {
                if (Constants.All == null)
                    Constants.All = GetAll();

                return Constants.All;
            }
        }

        public Modl()
        {
            store = new Store<C>(IdName);

            Fields = store.Fields;
            F = store.Fields;
            //Lazy = new DynamicFields(store);

            store.SetDefaults(this);
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

        public static C Get(int id, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return GetWhere(IdName, id, database, throwExceptionOnNotFound);
        }

        protected static C Get(Select<C> statement, bool throwExceptionOnNotFound = true)
        {
            
            return Get(DbAccess.ExecuteReader(statement), statement.DatabaseProvider, throwExceptionOnNotFound, true);
        }

        protected static C Get(DbDataReader reader, Database database, bool throwExceptionOnNotFound = true, bool singleRow = true)
        {
            var c = Modl<C>.New(database); //new C();

            if (c.store.Load(reader, throwExceptionOnNotFound, singleRow))
            {
                c.isNew = false;
                return c;
            }
            else
                return null;
        }

        public static C GetCached(int id, bool throwExceptionOnNotFound = true)
        {
            if (throwExceptionOnNotFound)
                return AllCached.Single(x => x.Id == id);
            else
                return AllCached.SingleOrDefault(x => x.Id == id);
        }

        protected static List<C> GetList(Select<C> statement)
        {
            var list = new List<C>();

            using (DbDataReader reader = DbAccess.ExecuteReader(statement))
            {
                while (!reader.IsClosed)
                {
                    C c = Get(reader, statement.DatabaseProvider, singleRow: false);

                    if (c != null)
                        list.Add(c);
                }
            }

            return list;
        }

        public static C GetWhere<T>(string field, T value, Database database = null, bool throwExceptionOnNotFound = true)
        {
            return GetWhere(database, throwExceptionOnNotFound, Tuple.Create(field, value));
        }

        public static C GetWhere<T>(params Tuple<string, T>[] fields)
        {
            return GetWhere(null, true, fields);
        }

        public static C GetWhere<T>(Database database = null, bool throwExceptionOnNotFound = true, params Tuple<string, T>[] fields)
        {
            var select = new Select<C>(database ?? DefaultDatabase);

            foreach (var field in fields)
                select.Where(field.Item1).EqualTo(field.Item2.ToString());

            return Get(select, throwExceptionOnNotFound);
        }

        public static List<C> GetAll(Database database = null)
        {
            return GetList(new Select<C>(database ?? DefaultDatabase));
        }

        public static List<C> GetAllWhere<T>(string field, T value, Database database = null)
        {
            return GetAllWhere(database, Tuple.Create(field, value));
        }
        
        public static List<C> GetAllWhere<T>(params Tuple<string, T>[] fields)
        {
            return GetAllWhere(null, fields);
        }

        public static List<C> GetAllWhere<T>(Database database = null, params Tuple<string, T>[] fields)
        {
            var select = new Select<C>(database ?? DefaultDatabase);

            foreach(var field in fields)
                select.Where(field.Item1).EqualTo(field.Item2.ToString());

            return GetList(select);
        }

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
            if (!Lazy.Dictionary.ContainsKey(name) || Lazy.Dictionary[name] == null)
                Lazy.Dictionary[name] = fetchCode();

            return Lazy.Dictionary[name];
        }

        protected void SetLazy<T>(string name, T value)
        {
            Lazy.Dictionary[name] = value;
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

            store.BaseAddSaveFields(statement);

            if (dbTransaction != null)
                BaseTransactionSave(statement, dbTransaction);
            else
                BaseSave(statement);

            LogSave();
        }

        protected Change<C> BaseGetSaveStatement()
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

        

        protected void BaseSave(Change<C> statement)
        {
            if (isDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", TableName, Id));

            if (statement is Insert<C>)
                store.Id = DbAccess.ExecuteScalar<int>(statement, Database.GetLastIdQuery());
            else
                DbAccess.ExecuteNonQuery(statement);

            isNew = false;
        }

        protected void BaseTransactionSave(Change<C> statement, Modl.DataAccess.DbTransaction trans)
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
            LogDelete();

            Delete<C> statement = new Delete<C>(Database);
            statement.Where(IdName).EqualTo(Id);

            DbAccess.ExecuteNonQuery(statement);

            isDeleted = true;
        }

        protected void LogSave()
        {
            //if (isNew)
            //    Log.Create(null, this);
            //else
            //    Log.Edit(null, this);
        }

        protected void LogDelete()
        {
            //Log.Delete(null, this);
        }

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

        
        protected static Tuple<string, T> Q<T>(string field, T value)
        {
            return Tuple.Create<string, T>(field, value);
        }

        public static IEnumerable<SelectListItem> SelectList<T>(string textField, bool firstRow = false,string firstRowText = "Any")
        {
            return SelectList<T>(AllCached, textField, firstRow,firstRowText);
        }

        public static IEnumerable<SelectListItem> SelectList<T>(IEnumerable<C> list, string textField, bool firstRow, string firstRowText = "Any")
        {
            List<SelectListItem> selectList =
                (from c in list
                 select new SelectListItem
                 {
                     Text = c.Fields.Dictionary[textField].ToString(),
                     Value = Helper.ConvertTo<T>(c.Id).ToString()
                 })
            .ToList();

            if (firstRow)
            {
                selectList.Insert(0,
                    new SelectListItem
                    {
                        //Selected = true,
                        Text = firstRowText,
                        Value = "0"
                    }
                );
            }   

            return selectList;
        }


        //public static explicit operator BcBase<C>(string id)
        //{
        //    return Get(Convert.ToInt32(id));
        //}

        //public static explicit operator string(BcBase<C> x)
        //{
        //    return x.Id.ToString();
        //}

        public Select<C> Select()
        {
            return new Select<C>(Database);
        }

        public Where<C, K> Where<K>(string key) //where K : Query<C, K>
        {
            return new Where<C, K>(key);
        }

        public Where<C, Select<C>> Where(string key)
        {
            return new Where<C, Select<C>>(key);
        }

        //public Where<C, Update<C>> Where(string key)
        //{
        //    return new Where<C, Update<C>>(key);
        //}

        //public Where<C, Select<C>> Where(string key) 
        //{
        //    return new Where<C, Select<C>>(key);
        //}
    }
}
