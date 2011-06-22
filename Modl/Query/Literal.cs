using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Modl
{
    public class Literal : IQuery
    {
        private string sql;

        public Literal(string sql)
        {
            this.sql = sql;
        }

        public override string ToString()
        {
            return sql;
        }

        public SqlCommand ToSqlCommand()
        {
            throw new NotImplementedException();
        }
    }
}
