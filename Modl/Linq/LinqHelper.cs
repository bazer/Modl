/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Modl.Exceptions;
using Modl.Fields;

namespace Modl.Linq
{
    internal class LinqHelper
    {
        internal static KeyValuePair<string, object> GetFields<T>(BinaryExpression node) where T : IModl<T>, new()
        {
            if (node.Left is ConstantExpression && node.Right is ConstantExpression)
                throw new InvalidQueryException("Unable to compare 2 constants.");

            if (node.Left is MemberExpression)
                return GetValues<T>(node.Left, node.Right);
            else
                return GetValues<T>(node.Right, node.Left);
        }

        internal static KeyValuePair<string, object> GetValues<T>(Expression field, Expression value) where T : IModl<T>, new()
        {
            return new KeyValuePair<string, object>((string)GetValue<T>(field), GetValue<T>(value));
        }

        internal static object GetValue<T>(Expression expression) where T : IModl<T>, new()
        {
            if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;
            else if (expression is MemberExpression)
                return Statics<T>.GetFieldName(((MemberExpression)expression).Member.Name);
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
