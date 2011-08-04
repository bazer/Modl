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

    public class Where<M, Q> : QueryPart<M>
        where M : Modl<M>, new() 
        where Q : Query<M, Q>
    {
        Q Query;
        string Key;
        object Value;
        Relation Relation;


        internal Where(Q query, string key)
        {
            this.Query = query;
            this.Key = key;
        }

        public Where(string key)
        {
            this.Key = key;
        }

        public Q EqualTo<V>(V value)
        {
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

        public override string GetCommandString(int counter)
        {
            return Query.DatabaseProvider.GetParameterComparison(Key, Relation, "w" + counter);

            //return string.Format("{0} {1} @{2}", Key, RelationToString(Relation), number);
        }

        public override IDataParameter GetCommandParameter(int counter)
        {
            return Query.DatabaseProvider.GetParameter("w" + counter, Value);

            //return new Tuple<string, object>("@" + number, Value);
        }
    }
}
