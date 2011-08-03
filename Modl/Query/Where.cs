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

    public class Where<C, T> : QueryPart<C> 
        where C : Modl<C>, new() 
        where T : Query<C, T>
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
            return SetAndReturn(value, Modl.Query.Relation.Equal);
        }

        public T NotEqualTo<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.NotEqual);
        }

        public T Like<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.Like);
        }

        public T GreaterThan<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.BiggerThan);
        }

        public T GreaterThanOrEqual<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.BiggerThanOrEqual);
        }

        public T LessThan<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.SmallerThan);
        }

        public T LessThanOrEqual<V>(V value)
        {
            return SetAndReturn(value, Modl.Query.Relation.SmallerThanOrEqual);
        }

        private T SetAndReturn<V>(V value, Relation relation)
        {
            this.Value = value;
            this.Relation = relation;

            return Query;
        }

        //public override string ToString()
        //{
        //    return string.Format("{0} {1} '{2}'", Key, RelationToString(Relation), Value.ToString());
        //}

        public override string GetCommandString(int number)
        {
            return Query.DatabaseProvider.GetCommandString(Key, Relation, number.ToString());

            //return string.Format("{0} {1} @{2}", Key, RelationToString(Relation), number);
        }

        public override IDbDataParameter GetCommandParameter(int number)
        {
            return Query.DatabaseProvider.GetCommandParameter(number.ToString(), Value);

            //return new Tuple<string, object>("@" + number, Value);
        }
    }
}
