using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Modl.Query
{
    public abstract class QueryPart<M> 
        where M : Modl<M>, new()
    {
        public abstract string GetCommandString(int number);
        public abstract IDataParameter GetCommandParameter(int number);

        public QueryPart()
        {
        }
    }
}
