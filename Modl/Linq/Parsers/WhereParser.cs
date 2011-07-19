using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Modl.Query;

namespace Modl.Linq.Parsers
{
    internal class WhereParser<T> : ExpressionVisitor where T : Modl<T>, new()
    {
        protected Select<T> select;

        internal WhereParser(Select<T> select)
        {
            this.select = select;
        }

        //internal void ParseWhere(Expression expression)
        //{
        //    Visit(expression);
        //}

        protected override Expression VisitBinary(BinaryExpression node)
        {
            //string value = ExpressionTreeHelpers.GetValueFromEqualsExpression(node, typeof(T), (((MemberExpression)node.Left).Member.Name));
            
            string field = Modl<T>.GetFieldName(((MemberExpression)node.Left).Member.Name);
            object value = ExpressionTreeHelpers.GetValueFromExpression(node.Right);

            select.Where(field).EqualTo(value);

            return base.VisitBinary(node);
        }
    }
}
