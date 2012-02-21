using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Linq;
using Modl.Query;
using System.Linq.Expressions;
using Modl.Cache;
using Modl.Fields;
using Modl.DataAccess;
using System.Data.Common;

namespace Modl
{
    public interface IDbModl : IModl
    {
        
    }

    public class DbModl<M> : Modl<M>
        where M : IDbModl, new()
    {
        
        internal static List<Table> Tables { get { return Statics<M>.Tables; } }

        public static Database DefaultDatabase { get { return Statics<M>.DefaultDatabase; } set { Statics<M>.DefaultDatabase = value; } }


        public static M New(object id, string databaseName)
        {
            return New(id, Config.GetDatabase(databaseName));
        }

        public static M New(object id, Database database)
        {
            var m = New(database);
            m.SetId(id);

            return m;
        }

        public static M New(string databaseName)
        {
            return New(Config.GetDatabase(databaseName));
        }

        public static M New(Database database)
        {
            var m = new M();
            var content = Statics<M>.AddInstance(m);

            if (database != null)
                content.Database = database;

            return m;
        }

        internal static M Load(DbDataReader reader, Database database)
        {
            var m = New(database);
            Statics<M>.Load(m, reader);

            return m;
        }

        public static bool Exists(object id, Database database = null)
        {
            return Get(id, database) != null;
        }

        public static bool Exists(Expression<Func<M, bool>> query, Database database = null)
        {
            return GetWhere(query, database) != null;
        }

        public static M Get(object id, Database database = null)
        {
            return new Select(database ?? Statics<M>.DefaultDatabase, Tables[0]).Where(Tables[0].PrimaryKeyName).EqualTo(id).AddJoins(Tables).Get<M>();


            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.Get(id, database ?? DefaultDatabase);
            //else


            //return new Select(database ?? Statics<M>.DefaultDatabase, Tables[0]).Where(DbModl<M>.IdName).EqualTo(id).Get<M>();

            //return new M();
        }

        public static M GetWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.GetWhere(query, database ?? DefaultDatabase);
            //else
            return new Select(database ?? Statics<M>.DefaultDatabase, Tables[0], query).AddJoins(Tables).Get<M>();

            //return Get(new Select<M>(database ?? DefaultDatabase, query), throwExceptionOnNotFound);
        }

        public static IEnumerable<M> GetAll(Database database = null)
        {
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.GetAll(database ?? DefaultDatabase);
            //else
            return new Select(database ?? Statics<M>.DefaultDatabase, Tables[0]).AddJoins(Tables).GetAll<M>();

            //return StaticCache<M, IdType>.GetAll(new Select<M, IdType>(database ?? DefaultDatabase));
            //return new Select<M, IdType>(database ?? DefaultDatabase).GetList(true);
            //return GetList(new Select<M>(database ?? DefaultDatabase));
        }

        public static IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.GetAllWhere(query, database ?? DefaultDatabase);
            //else
            return new Select(database ?? Statics<M>.DefaultDatabase, Tables[0], query).AddJoins(Tables).GetAll<M>();

            //return new Select<M, IdType>(database ?? DefaultDatabase, query).GetList(true);
            //return GetList(new Select<M>(database ?? DefaultDatabase, query));
        }

        public static bool Delete(object id, Database database = null)
        {
            var db = database ?? DefaultDatabase;

            Delete statement = new Delete(db, Tables[0]);
            statement.Where(IdName).EqualTo(id);

            return DbAccess.ExecuteNonQuery(statement);
        }

        public static bool DeleteAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            return DbAccess.ExecuteNonQuery(new Delete(database ?? DefaultDatabase, Tables[0], query));
        }

        public static bool DeleteAll(Database database = null)
        {
            return DbAccess.ExecuteNonQuery(new Delete(database ?? DefaultDatabase, Tables[0]));
        }

        public static IQueryable<M> Query(Database database = null)
        {
            return new LinqQuery<M>(database ?? DefaultDatabase);
        }

        
    }

    public static class DbModlExtensions
    {
        public static Database Database<M>(this M m) where M : IDbModl, new()
        {
            return m.GetContent().Database;
        }

        public static bool WriteToDb<M>(this M m) where M : IDbModl, new()
        {
            var content = m.GetContent();
            Statics<M>.ReadFromEmptyProperties(m);

            if (content.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (!Statics<M>.IsDirty(m))
                return false;

            object keyValue = null;
            Type parentType = null;

            foreach (var t in Statics<M>.Tables)
            {
                if (keyValue != null && parentType != null && t.ForeignKeys.Count != 0)
                {
                    var fk = t.ForeignKeys.Where(x => x.Value == parentType).Select(x => x.Key).SingleOrDefault();

                    if (fk != null)
                        content.SetValue(fk, keyValue);
                }

                Change query;

                if (content.IsNew)
                    query = new Insert(content.Database, t);
                else
                    query = new Update(content.Database, t).Where(t.PrimaryKeyName).EqualTo(content.GetValue<object>(t.PrimaryKeyName));

                foreach (var f in t.Fields)
                {
                    var field = content.Fields[f.Key];
                    //if (field.Value.Type.GetInterface("IModl") != null) // && !(field.Value.Value is int))
                    //{
                    //    var m = (M)field.Value.Value;

                    //    if (m == null)
                    //        yield return new KeyValuePair<string, object>(field.Key, null);
                    //    //statement.With(field.Key, null);
                    //    else
                    //    {
                    //        if (m.IsDirty())
                    //            throw new Exception("Child " + m + " is dirty");

                    //        yield return new KeyValuePair<string, object>(field.Key, m.GetId());
                    //        //statement.With(field.Key, value.Id);
                    //    }
                    //}
                    if (field.IsDirty && (!content.AutomaticId || !t.HasKey || f.Key != t.PrimaryKeyName))
                        query.With(f.Key, field.Value);
                }

                

                if (content.IsNew && content.AutomaticId && t.HasKey)
                    keyValue = DbAccess.ExecuteScalar(t.PrimaryKeyType, query, content.Database.GetLastIdQuery());
                else
                    DbAccess.ExecuteScalar(typeof(object), query);

                if (keyValue != null && t.Keys.Count != 0)
                    content.SetValue(t.PrimaryKeyName, keyValue);

                parentType = t.Type;
            }

            
            content.IsNew = false;
            content.ResetFields();

            Statics<M>.WriteToEmptyProperties(m);

            return true;
        }

        public static bool DeleteFromDb<M>(this M m) where M : IDbModl, new()
        {
            var result = DbModl<M>.Delete(m.GetId(), m.Database());
            
            if (result)
                m.GetContent().IsDeleted = true;

            return result;
        }

        public static Select AddJoins(this Select s, List<Table> tables)
        {
            Table parent = tables[0];
            foreach (var t in tables.Skip(1))
            {
                if (t.ForeignKeys.Count != 0)
                {
                    var fk = t.ForeignKeys.Where(x => x.Value == parent.Type).Select(x => x.Key).SingleOrDefault();

                    if (fk != null)
                        s.InnerJoin(t.Name).Where(fk).EqualTo(parent.PrimaryKeyName);
                }
            }

            return s;
        }
    }
}
