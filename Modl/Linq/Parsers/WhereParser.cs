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
            var fields = GetFields(node);
            var where = select.Where(fields.Key);

            if (node.NodeType == ExpressionType.Equal)
                where.EqualTo(fields.Value);
            else
                throw new NotImplementedException("Operation not implemented");


            
            //string field = Modl<T>.GetFieldName(((MemberExpression)node.Left).Member.Name);
            //object value = ExpressionTreeHelpers.GetValueFromExpression(node.Right);

            

            return base.VisitBinary(node);
        }

        protected KeyValuePair<string, object> GetFields(BinaryExpression node)
        {
            if (node.Left is ConstantExpression && node.Right is ConstantExpression)
                throw new InvalidQueryException("Unable to compare 2 constants.");

            if (node.Left is MemberExpression)
                return GetValues(node.Left, node.Right);
            else
                return GetValues(node.Right, node.Left);
        }

        protected KeyValuePair<string, object> GetValues(Expression field, Expression value)
        {
            return new KeyValuePair<string, object>((string)GetValue(field), GetValue(value));
        }

        protected object GetValue(Expression expression)
        {
            if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;
            else
                return Modl<T>.GetFieldName(((MemberExpression)expression).Member.Name);
        }
    }
}
