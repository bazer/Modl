/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

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
using System.Data;

namespace Modl.Query
{
    internal enum Relation
    {
        Equal,
        NotEqual,
        Like,
        BiggerThan,
        BiggerThanOrEqual,
        SmallerThan,
        SmallerThanOrEqual
    }

    public class Where<Q> : QueryPart
        //where M : IDbModl, new()
        where Q : Query<Q>
    {
        Q Query;
        string Key;
        object Value;
        Relation Relation;
        bool IsValue = true;

        internal Where(Q query, string key, bool isValue = true)
        {
            this.Query = query;
            this.Key = key;
            this.IsValue = isValue;
        }

        public Where(string key)
        {
            this.Key = key;
        }

        public Q EqualTo<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.Equal);
        }

        public Q EqualTo<V>(V value, bool isValue)
        {
            this.IsValue = isValue;
            return SetAndReturn(value, Modl.Query.Relation.Equal);
        }

        public Q NotEqualTo<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.NotEqual);
        }

        public Q Like<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.Like);
        }

        public Q GreaterThan<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.BiggerThan);
        }

        public Q GreaterThanOrEqual<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.BiggerThanOrEqual);
        }

        public Q LessThan<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.SmallerThan);
        }

        public Q LessThanOrEqual<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.SmallerThanOrEqual);
        }

        private Q SetAndReturn<V>(V value, Relation relation)
        {
            this.Value = value;
            this.Relation = relation;

            return Query;
        }

        //public override string ToString()
        //{
        //    return string.Format("{0} {1} '{2}'", Key, RelationToString(Relation), Value.ToString());
        //}

        public override Sql GetCommandString(Sql sql, string prefix, int number)
        {
            if (IsValue)
                return Query.DatabaseProvider.GetParameterComparison(sql, Key, Relation, prefix + "w" + number);
            else
                return sql.AddFormat("{0} {1} {2}", Key, Relation.ToSql(), Value.ToString());
                
            
            //return Query.DatabaseProvider.GetParameterComparison(sql, Key, Relation, Value.ToString());

            //return string.Format("{0} {1} @{2}", Key, RelationToString(Relation), number);
        }

        public override Sql GetCommandParameter(Sql sql, string prefix, int number)
        {
            if (IsValue)
                return Query.DatabaseProvider.GetParameter(sql, prefix + "w" + number, Value);
            else
                return sql;

            //return new Tuple<string, object>("@" + number, Value);
        }
    }
}
