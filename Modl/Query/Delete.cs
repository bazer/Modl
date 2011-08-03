using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public class Delete<C> : Query<C, Delete<C>> where C : Modl<C>, new()
    {
        public Delete(Database database) : base(database) { }

        public override string ToString()
        {
            return string.Format("DELETE FROM {0} \r\nWHERE \r\n{1}", Modl<C>.TableName, QueryPartsToString());
        }
    }
}
