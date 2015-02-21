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
using Modl.Db.Query;
using Modl.Exceptions;

namespace Modl.Db.Linq.Parsers
{
    internal class WhereParser<M, Q> : ExpressionVisitor
        where M : IDbModl, new()
        where Q : Query<Q>
    {
        protected Query<Q> select;

        internal WhereParser(Query<Q> select)
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
