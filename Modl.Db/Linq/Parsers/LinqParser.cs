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

namespace Modl.Db.Linq.Parsers
{
    internal class LinqParser<M, Q> : ExpressionVisitor
        where M : IDbModl, new()
        where Q : Query<Q>
    {
        protected Query<Q> select;
        protected WhereParser<M, Q> whereParser;

        internal LinqParser(Query<Q> select) 
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
