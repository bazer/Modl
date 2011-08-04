using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Modl.Query
{
    public abstract class QueryPart<C> where C : Modl<C>, new()
    {
        public abstract string GetCommandString(int number);
        public abstract IDataParameter GetCommandParameter(int number);

        public QueryPart()
        {
        }
    }
}
