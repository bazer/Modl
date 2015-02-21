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
using System.Data;

namespace Modl.Db.Query
{
    internal enum JoinType
    {
        Inner,
        Outer
    }

    public class Join<Q>
        where Q : Query<Q>
    {
        Q Query;
        string TableName;
        JoinType Type;

        protected List<Where<Q>> whereList = new List<Where<Q>>();


        internal Join(Q query, string tableName, JoinType type)
        {
            this.Query = query;
            this.TableName = tableName;
            this.Type = type;
        }

        public Where<Q> Where(string key)
        {
            var where = new Where<Q>(Query, key, false);
            whereList.Add(where);

            return where;
        }

        public Sql GetSql(Sql sql, string tableAlias)
        {
            int length = whereList.Count;
            if (length == 0)
                return sql;

            if (Type == JoinType.Inner)
                sql.AddText("INNER JOIN ");
            else if (Type == JoinType.Outer)
                sql.AddText("LEFT OUTER JOIN ");
            else
                throw new NotImplementedException("Wrong JoinType: " + Type);

            sql.AddFormat("{0} {1} ON ", TableName, tableAlias);

            for (int i = 0; i < length; i++)
            {
                //whereList[i].GetCommandParameter(sql, paramPrefix, i);
                whereList[i].GetCommandString(sql, "", i);

                if (i + 1 < length)
                    sql.AddText(" AND ");
            }

            sql.AddText(" \r\n");

            return sql;
        }

        //public override Sql GetCommandString(Sql sql, string prefix, int number)
        //{
        //    return Query.DatabaseProvider.GetParameterComparison(sql, Key, Relation, prefix + "w" + number);
        //}

        //public override Sql GetCommandParameter(Sql sql, string prefix, int number)
        //{
        //    return Query.DatabaseProvider.GetParameter(sql, prefix + "w" + number, Value);
        //}
    }
}
