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
        IEnumerable<IDataParameter> parameters;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }

        public Literal(Database database, string sql)
        {
            this.provider = database;
            this.sql = sql;
            this.parameters = new List<IDataParameter>();
        }

        public Literal(Database database, string sql, IEnumerable<IDataParameter> parameters)
        {
            this.provider = database;
            this.sql = sql;
            this.parameters = parameters;
        }

        public Tuple<string, IEnumerable<IDataParameter>> ToSql()
        {
            return new Tuple<string, IEnumerable<IDataParameter>>(sql, parameters);
        }

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }

        //public IEnumerable<IDataParameter> QueryPartsParameters()
        //{
        //    return new List<IDataParameter>();
        //}
    }
}
