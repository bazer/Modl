﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Query
{
    public abstract class QueryPart<C> where C : Modl<C>, new()
    {
        public QueryPart()
        {
        }
    }
}
