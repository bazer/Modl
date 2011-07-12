using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;

namespace Modl
{
    public class Select<C> : Query<C, Select<C>> where C : Modl<C>, new()
    {
        public Select(DatabaseProvider database) : base(database) { }

        public override string ToString()
        {
            return string.Format("SELECT * FROM {0}\r\nWHERE\r\n{1}", Modl<C>.TableName, QueryPartsToString());
        }
    }
}
