using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;

namespace Modl
{
    public interface IQuery
    {
        DatabaseProvider DatabaseProvider { get; }
        IDbCommand ToDbCommand();
    }

    public abstract class Query<C, T> : IQuery 
        where C : Modl<C>, new()
        where T : Query<C, T>
    {
        protected List<QueryPart<C>> queryParts = new List<QueryPart<C>>();
        protected ModlBase owner;
        protected DatabaseProvider provider;
        public DatabaseProvider DatabaseProvider { get { return provider; } }

        public Query()
        { 
        }

        public Query(string databaseName)
        {
            provider = Config.DatabaseProviders[databaseName];
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

        public T And()
        {
            return (T)this;
        }

        public T And(T where)
        {
            return (T)this;
        }

        public T Or()
        {
            return (T)this;
        }

        public T Or(T where)
        {
            return (T)this;
        }

        protected string QueryPartsToString()
        {
            return string.Join("\r\n", queryParts.Select(x => x.ToString()));
        }

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }
    }
}
