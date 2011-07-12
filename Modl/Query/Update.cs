using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;

namespace Modl
{
    public class Update<C> : Change<C> where C : Modl<C>, new()
    {
        public Update(DatabaseProvider database) : base(database) { }

        protected string ValuesToString()
        {
            return string.Join(",", ChangeValues.Select(x => x.Key + "='" + x.Value + "'"));
        }

        public override string ToString()
        {
            return string.Format("UPDATE {0} SET {1}\r\nWHERE\r\n{2}", Modl<C>.TableName, ValuesToString(), QueryPartsToString());
        }
    }
}
