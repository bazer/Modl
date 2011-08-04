using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Modl.Query;
using Modl.Exceptions;

namespace Modl.Linq.Parsers
{
    internal class WhereParser<M, Q> : ExpressionVisitor 
        where M : Modl<M>, new()
        where Q : Query<M, Q>
    {
        protected Query<M, Q> select;

        internal WhereParser(Query<M, Q> select)
        {
            this.select = select;
        }

        //internal void ParseWhere(Expression expression)
        //{
        //    Visit(expression);
        //}

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var fields = LinqHelper.GetFields<M>(node);

            //IWhere<Select<M>> where = ((Select<M>)select).Where(fields.Key);
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
