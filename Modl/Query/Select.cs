using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    public class Select<C> : Query<C, Select<C>> where C : Modl<C>, new()
    {
        public Select(string databaseName) : base(databaseName) { }

        public override string ToString()
        {
            return string.Format("SELECT * FROM {0}\r\nWHERE\r\n{1}", Modl<C>.TableName, QueryPartsToString());
        }
    }
}
