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

namespace Modl
{
    public interface IDbModl : IModl
    {
        
    }

    public class DbModl<M> : Modl<M>
        where M : IDbModl, new()
    {
        
        internal static string Table { get { return Statics<M>.TableName; } }

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
            content.instanceDbProvider = database;

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
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.Get(id, database ?? DefaultDatabase);
            //else
            return new Select<M>(database ?? Statics<M>.DefaultDatabase).Where(DbModl<M>.IdName).EqualTo(id).Get();

            //return new M();
        }

        public static M GetWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.GetWhere(query, database ?? DefaultDatabase);
            //else
            return new Select<M>(database ?? Statics<M>.DefaultDatabase, query).Get();

            //return Get(new Select<M>(database ?? DefaultDatabase, query), throwExceptionOnNotFound);
        }

        public static IEnumerable<M> GetAll(Database database = null)
        {
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.GetAll(database ?? DefaultDatabase);
            //else
            return new Select<M>(database ?? Statics<M>.DefaultDatabase).GetAll();

            //return StaticCache<M, IdType>.GetAll(new Select<M, IdType>(database ?? DefaultDatabase));
            //return new Select<M, IdType>(database ?? DefaultDatabase).GetList(true);
            //return GetList(new Select<M>(database ?? DefaultDatabase));
        }

        public static IEnumerable<M> GetAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            //if (CacheLevel == CacheLevel.On)
            //    return StaticCache<M, IdType>.GetAllWhere(query, database ?? DefaultDatabase);
            //else
            return new Select<M>(database ?? Statics<M>.DefaultDatabase, query).GetAll();

            //return new Select<M, IdType>(database ?? DefaultDatabase, query).GetList(true);
            //return GetList(new Select<M>(database ?? DefaultDatabase, query));
        }

        public static bool Delete(object id, Database database = null)
        {
            var db = database ?? DefaultDatabase;

            Delete<M> statement = new Delete<M>(db);
            statement.Where(IdName).EqualTo(id);

            return DbAccess.ExecuteNonQuery(statement);
        }

        public static bool DeleteAllWhere(Expression<Func<M, bool>> query, Database database = null)
        {
            return DbAccess.ExecuteNonQuery(new Delete<M>(database ?? DefaultDatabase, query));
        }

        public static bool DeleteAll(Database database = null)
        {
            return DbAccess.ExecuteNonQuery(new Delete<M>(database ?? DefaultDatabase));
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

            if (content.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", Statics<M>.TableName, m.GetId()));

            if (!content.IsDirty)
                return false;

            Change<M> query;

            if (content.IsNew)
                query = new Insert<M>(m.Database());
            else
                query = new Update<M>(m.Database()).Where(Statics<M>.IdName).EqualTo(m.GetId());
            
            foreach (var f in content.GetFields(!content.AutomaticId))
                query.With(f.Key, f.Value);

            if (content.IsNew && content.AutomaticId)
                m.SetId(DbAccess.ExecuteScalar(Statics<M>.IdType, query, content.Database.GetLastIdQuery()));
            else
                 DbAccess.ExecuteScalar(typeof(object), query, content.Database.GetLastIdQuery());
            
            content.IsNew = false;
            content.ResetFields();

            return true;
        }

        public static bool DeleteFromDb<M>(this M m) where M : IDbModl, new()
        {
            var result = DbModl<M>.Delete(m.GetId(), m.Database());
            
            if (result)
                m.GetContent().IsDeleted = true;

            return result;
        }
    }
}
