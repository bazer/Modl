using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public interface IQuery
    {
        Database DatabaseProvider { get; }
        IDbCommand ToDbCommand();
        Tuple<string, IEnumerable<IDataParameter>> ToSql();
        //IEnumerable<IDataParameter> QueryPartsParameters();
    }

    public abstract class Query<C, T> : IQuery
        where C : Modl<C>, new()
        where T : Query<C, T>
    {
        protected List<Where<C, T>> whereList = new List<Where<C, T>>();
        protected ModlBase owner;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }
        public abstract Tuple<string, IEnumerable<IDataParameter>> ToSql();

        public Query()
        {
        }

        public Query(Database database)
        {
            provider = database;
        }

        public Query(C owner)
        {
            this.owner = owner;
        }

        public Where<C, T> Where(string key)
        {
            var where = new Where<C, T>((T)this, key);
            whereList.Add(where);

            return where;
        }

        protected Tuple<string, IEnumerable<IDataParameter>> GetWhere()
        {
            if (whereList.Count == 0)
                return new Tuple<string, IEnumerable<IDataParameter>>(string.Empty, new List<IDataParameter>());

            int i = 0, j = 0;
            return new Tuple<string, IEnumerable<IDataParameter>>("WHERE \r\n" +
                string.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(i++))),
                whereList.Select(x => x.GetCommandParameter(j++)));
        }

        //protected string QueryPartsToString()
        //{
        //    int i = 0;
        //    return string.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(i++)));
        //}

        //public IEnumerable<IDataParameter> QueryPartsParameters()
        //{
        //    int i = 0;
        //    return whereList.Select(x => x.GetCommandParameter(i++));
        //}

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }
    }
}
