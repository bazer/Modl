using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public class Literal : IQuery
    {
        private string sql;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }
        
        public Literal(string databaseName, string sql)
        {
            this.sql = sql;
            provider = Config.GetDatabase(databaseName);
        }

        public override string ToString()
        {
            return sql;
        }

        public IDbCommand ToDbCommand()
        {
            throw new NotImplementedException();
        }
    }
}
