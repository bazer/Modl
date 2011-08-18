/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

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
using System.Data.SqlClient;
using System.Data;
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public interface IQuery
    {
        Database DatabaseProvider { get; }
        IDbCommand ToDbCommand();
        Sql ToSql(string paramPrefix);
        int ParameterCount { get; }
        //Where<C, T> Where(string key);
        //IEnumerable<IDataParameter> QueryPartsParameters();
    }

    public abstract class Query<M, IdType, Q> : IQuery
        where M : Modl<M, IdType>, new()
        where Q : Query<M, IdType, Q>
    {
        protected List<Where<M, IdType, Q>> whereList = new List<Where<M, IdType, Q>>();
        protected M owner;
        protected Database provider;
        public Database DatabaseProvider { get { return provider; } }
        public abstract Sql ToSql(string paramPrefix);
        public abstract int ParameterCount { get; }

        

        public Query()
        {
        }

        public Query(Database database)
        {
            provider = database;
        }

        public Query(M owner)
        {
            this.owner = owner;
        }

        public Where<M, IdType, Q> Where(string key)
        {
            var where = new Where<M, IdType, Q>((Q)this, key);
            whereList.Add(where);

            return where;
        }

        protected Sql GetWhere(Sql sql, string paramPrefix)
        {
            int length = whereList.Count;
            if (length == 0)
                return sql; 

            sql.AddText("WHERE \r\n");
            
            for (int i=0; i<length; i++)
            {
                whereList[i].GetCommandParameter(sql, paramPrefix, i);
                whereList[i].GetCommandString(sql, paramPrefix, i);

                if (i + 1 < length)
                    sql.AddText(" AND \r\n");
            }

            return sql;

                //.AddParameters(whereList.Select(x => x.GetCommandParameter(paramPrefix, j++)).ToArray())
                
                //.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(paramPrefix, i++)).ToArray());

            //return new Sql("WHERE \r\n" +
            //    string.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(paramPrefix, i++))),
            //    whereList.Select(x => x.GetCommandParameter(paramPrefix, j++)).ToArray());
        }

        //protected string QueryPartsToString()
        //{
        //    int i = 0;
        //    return string.Join(" AND \r\n", whereList.Select(x => x.GetCommandString(i++)));
        //}

        //public IEnumerable<IDataParameter> QueryPartsParameters()
        //{
        //    int i = 0;
        //    return whereList.Select(x => x.GetCommandParameter(i++));
        //}

        public IDbCommand ToDbCommand()
        {
            return DatabaseProvider.ToDbCommand(this);
        }

        public override string ToString()
        {
            var sql = ToSql(string.Empty);
            return sql.Text + "; " + string.Join(", ", sql.Parameters.Select(x => x.ParameterName + ": " + x.Value));

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(sql.Item1 + "; ");


            //foreach (var param in sql.Item2)
            //    sb.AppendLine(param.ParameterName + ": " + param.Value);

            //return sb.ToString();
        }
    }
}
