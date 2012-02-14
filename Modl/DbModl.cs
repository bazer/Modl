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
    public interface IDbModl<M> : IModl<M>
    {
        
    }

    public static class DbModl<M>
        where M : IDbModl<M>, new()
    {
        internal static string IdName { get { return Statics<M>.IdName; } }
        internal static string Table { get { return Statics<M>.TableName; } }

        public static Database DefaultDatabase { get { return Statics<M>.DefaultDatabase; } set { Statics<M>.DefaultDatabase = value; } }


        public static M New()
        {
            var m = new M();
            var content = Statics<M>.AddInstance(m);

            return m;
        }

        public static M New(object id)
        {
            var m = New();
            m.SetId(id);

            return m;
        }

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
        internal static Content<M> GetContent<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            if (m == null)
                throw new NullReferenceException("Modl object is null");

            return Statics<M>.GetContents(m);
        }

       
        public static bool IsNew<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            return m.GetContent().IsNew; 
        }

        public static bool IsDeleted<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            return m.GetContent().IsDeleted;
        }

        public static bool IsDirty<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            return m.GetContent().IsDirty;
        }

        public static Database Database<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            return m.GetContent().Database;
        }

        public static bool WriteToDb<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            if (m.IsDeleted())
                throw new Exception(string.Format("Trying to save a deleted object. Table: {0}, Id: {1}", Statics<M>.TableName, m.GetId()));

            if (!m.IsDirty())
                return false;

            Change<M> query;

            if (m.IsNew())
                query = new Insert<M>(m.Database());
            else
                query = new Update<M>(m.Database()).Where(Statics<M>.IdName).EqualTo(m.GetId());

            var content = m.GetContent();
            foreach (var f in content.GetFields(!content.AutomaticId))
                query.With(f.Key, f.Value);

            var newId = DbAccess.ExecuteScalar(Statics<M>.IdType, query, content.Database.GetLastIdQuery());
            
            if (newId != null)
                m.SetId(newId);

            content.IsNew = false;
            content.ResetFields();

            return true;
        }

        public static bool DeleteFromDb<M>(this IDbModl<M> m) where M : IDbModl<M>, new()
        {
            var result = DbModl<M>.Delete(m.GetId(), m.Database());
            
            if (result)
                m.GetContent().IsDeleted = true;

            return result;
        }
    }
}
