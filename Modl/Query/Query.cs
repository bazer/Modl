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
        IEnumerable<IDbDataParameter> QueryPartsParameters();
    }

    public abstract class Query<C, T> : IQuery
        where C : Modl<C>, new()
        where T : Query<C, T>
    {
        protected List<QueryPart<C>> queryParts = new List<QueryPart<C>>();
        protected ModlBase owner;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }

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
            queryParts.Add(where);

            return where;
        }

        protected string QueryPartsToString()
        {
            int i = 0;
            return string.Join(" AND \r\n", queryParts.Select(x => x.GetCommandString(i++)));
        }

        public IEnumerable<IDbDataParameter> QueryPartsParameters()
        {
            int i = 0;
            return queryParts.Select(x => x.GetCommandParameter(i++));
        }

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }
    }
}
