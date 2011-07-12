﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;

namespace Modl
{
    public class Insert<C> : Change<C> where C : Modl<C>, new()
    {
        public Insert(DatabaseProvider database) : base(database) { }

        protected string ValuesToString()
        {
            return string.Format("({0}) VALUES ({1})",
                string.Join(",", ChangeValues.Keys),
                string.Join(",", ChangeValues.Values.Select(x => "'" + x + "'"))
            );
        }

        public override string ToString()
        {
            return string.Format("INSERT INTO {0} {1}", Modl<C>.TableName, ValuesToString());
        }
    }
}
