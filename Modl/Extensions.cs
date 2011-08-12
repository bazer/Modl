/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Query;

namespace Modl
{
    internal static class Extensions
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
