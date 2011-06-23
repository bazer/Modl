using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    public class Delete<C> : Query<C, Delete<C>> where C : Modl<C>, new()
    {
        public Delete(string databaseName) : base(databaseName) { }

        public override string ToString()
        {
            return string.Format("DELETE FROM {0}\r\nWHERE\r\n{1};", Modl<C>.TableName, QueryPartsToString());
        }
    }
}
