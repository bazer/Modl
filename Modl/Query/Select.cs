using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;
using Modl.Linq;
using System.Linq.Expressions;
using System.Collections;
using Modl.Linq.Parsers;

namespace Modl.Query
{
    public class Select<C> : Query<C, Select<C>>
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
            var parser = new LinqParser<C>(this);
            parser.ParseTree(expression);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT * FROM {0} \r\n", Modl<C>.TableName);

            if (queryParts.Count > 0)
                sb.AppendFormat("WHERE\r\n {0}", QueryPartsToString());

            return sb.ToString();
        }
    }
}
