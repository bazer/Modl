using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Modl.Query;

namespace Modl.Linq.Parsers
{
    internal class LinqParser<M, Q> : ExpressionVisitor 
        where M : Modl<M>, new()
        where Q : Query<M, Q>
    {
        protected Query<M, Q> select;
        protected WhereParser<M, Q> whereParser;

        internal LinqParser(Query<M, Q> select) 
        {
            this.select = select;
            whereParser = new WhereParser<M, Q>(select);
        }

        internal void ParseTree(Expression expression)
        {
            Visit(Evaluator.PartialEval(expression));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where")
            {
                whereParser.Visit(node.Arguments[1]);
            }
            
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            whereParser.Visit(node);
            
            return base.VisitBinary(node);
        }

        //protected override Expression VisitLambda<T>(Expression<T> node)
        //{
        //    return base.VisitLambda<T>(node);
        //}
    }
}
