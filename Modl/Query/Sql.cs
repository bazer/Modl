using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Modl.Query
{
    public class Sql
    {
        public string Text;
        public IDataParameter[] Parameters;

        public Sql()
        {
            this.Text = string.Empty;
            this.Parameters = new IDataParameter[0];
        }

        public Sql(string text, params IDataParameter[] parameters)
        {
            this.Text = text;
            this.Parameters = parameters;
        }
    }
}
