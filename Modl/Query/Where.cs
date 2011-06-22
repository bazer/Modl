using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
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

    public class Where<C, T> : QueryPart<C> where C : Modl<C>, new()
    {
        T Query;
        string Key;
        object Value;
        Relation Relation;


        internal Where(T query, string key)
        {
            this.Query = query;
            this.Key = key;
        }

        public Where(string key)
        {
            this.Key = key;
        }

        public T EqualTo<V>(V value)
        {
            return SetAndReturn(value, Modl.Relation.Equal);
        }

        public T NotEqualTo<V>(V value)
        {
            return SetAndReturn(value, Modl.Relation.NotEqual);
        }

        public T Like<V>(V value)
        {
            return SetAndReturn(value, Modl.Relation.Like);
        }

        private T SetAndReturn<V>(V value, Relation relation)
        {
            this.Value = value;
            this.Relation = relation;

            return Query;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} '{2}'", Key, RelationToString(Relation), Value.ToString());
        }

        private string RelationToString(Relation relation)
        {
            if (relation == Modl.Relation.Equal)
                return "=";
            else if (relation == Modl.Relation.NotEqual)
                return "<>";
            else if (relation == Modl.Relation.Like)
                return "LIKE";
            else if (relation == Modl.Relation.BiggerThan)
                return ">";
            else if (relation == Modl.Relation.BiggerThanOrEqual)
                return ">=";
            else if (relation == Modl.Relation.SmallerThan)
                return "<";
            else if (relation == Modl.Relation.SmallerThanOrEqual)
                return "<=";

            return null;
        }
    }
}
