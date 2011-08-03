using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Query;

namespace Modl
{
    public static class Extensions
    {
        internal static string ToSql(this Relation relation)
        {
            if (relation == Modl.Query.Relation.Equal)
                return "=";
            else if (relation == Modl.Query.Relation.NotEqual)
                return "<>";
            else if (relation == Modl.Query.Relation.Like)
                return "LIKE";
            else if (relation == Modl.Query.Relation.BiggerThan)
                return ">";
            else if (relation == Modl.Query.Relation.BiggerThanOrEqual)
                return ">=";
            else if (relation == Modl.Query.Relation.SmallerThan)
                return "<";
            else if (relation == Modl.Query.Relation.SmallerThanOrEqual)
                return "<=";

            return null;
        }
    }
}
