using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;

namespace Modl
{
    public class Literal : IQuery
    {
        private string sql;
        protected DatabaseProvider provider;
        public DatabaseProvider DatabaseProvider { get { return provider; } }
        
        public Literal(string databaseName, string sql)
        {
            this.sql = sql;
            provider = Config.DatabaseProviders[databaseName];
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
