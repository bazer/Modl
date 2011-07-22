using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Modl.Query;
using Modl.Exceptions;

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
            var fields = LinqHelper.GetFields<T>(node);
            var where = select.Where(fields.Key);

            if (node.NodeType == ExpressionType.Equal)
                where.EqualTo(fields.Value);
            else if (node.NodeType == ExpressionType.NotEqual)
                where.NotEqualTo(fields.Value);
            else if (node.NodeType == ExpressionType.GreaterThan)
                where.GreaterThan(fields.Value);
            else if (node.NodeType == ExpressionType.GreaterThanOrEqual)
                where.GreaterThanOrEqual(fields.Value);
            else if (node.NodeType == ExpressionType.LessThan)
                where.LessThan(fields.Value);
            else if (node.NodeType == ExpressionType.LessThanOrEqual)
                where.LessThanOrEqual(fields.Value);
            else
                throw new NotImplementedException("Operation not implemented");

            return base.VisitBinary(node);
        }
    }
}
