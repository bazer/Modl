using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Modl.Exceptions;

namespace Modl.Linq
{
    internal class LinqHelper
    {
        internal static KeyValuePair<string, object> GetFields<T>(BinaryExpression node) where T : Modl<T>, new()
        {
            if (node.Left is ConstantExpression && node.Right is ConstantExpression)
                throw new InvalidQueryException("Unable to compare 2 constants.");

            if (node.Left is MemberExpression)
                return GetValues<T>(node.Left, node.Right);
            else
                return GetValues<T>(node.Right, node.Left);
        }

        internal static KeyValuePair<string, object> GetValues<T>(Expression field, Expression value) where T : Modl<T>, new()
        {
            return new KeyValuePair<string, object>((string)GetValue<T>(field), GetValue<T>(value));
        }

        internal static object GetValue<T>(Expression expression) where T : Modl<T>, new()
        {
            if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;
            else if (expression is MemberExpression)
                return Modl<T>.GetFieldName(((MemberExpression)expression).Member.Name);
            //else if (expression.NodeType == ExpressionType.Lambda)
            //    return GetValue<T>(((LambdaExpression)expression).Body);
            else
                throw new InvalidQueryException("Value is not a member or constant.");
        }

        //internal static object GetValue<T>(Expression<T> expression) where T : Modl<T>, new()
        //{
        //    if (expression.NodeType == ExpressionType.Lambda)
        //        return GetValue<T>(((LambdaExpression)expression).Body);
        //    else
        //        return GetValue<T>(expression);
        //}
    }
}
