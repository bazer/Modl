using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;
using Modl.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Modl.Query
{
    public class Select<C> : Query<C, Select<C>> //, IQueryable<C>
        where C : Modl<C>, new()
    {
        Expression expression;

        public Select(Database database)
            : base(database)
        {
            expression = Expression.Constant(this);
        }

        public Select(Database database, Expression expression)
            : base(database)
        {
            this.expression = expression;
            //var parser = new LinqParser();
            //parser.Visit(expression);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT * FROM {0}\r\n", Modl<C>.TableName);

            if (queryParts.Count > 0)
                sb.AppendFormat("WHERE\r\n{1}", Modl<C>.TableName, QueryPartsToString());

            return sb.ToString();
        }

        //public IEnumerator<C> GetEnumerator()
        //{
        //    return ((IEnumerable<C>)provider.Execute(this.expression)).GetEnumerator();
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        //}

        //public Type ElementType
        //{
        //    get { return typeof(C); }
        //}

        //public System.Linq.Expressions.Expression Expression
        //{
        //    get { return expression; }
        //}

        //public IQueryProvider Provider
        //{
        //    get { return provider; }
        //}
    }
}
