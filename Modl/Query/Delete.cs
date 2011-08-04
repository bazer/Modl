using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;
using System.Data;
using System.Linq.Expressions;
using Modl.Linq.Parsers;

namespace Modl.Query
{
    public class Delete<M> : Query<M, Delete<M>> 
        where M : Modl<M>, new()
    {
        Expression expression;

        public Delete(Database database) : base(database) { }

        public Delete(Database database, Expression expression)
            : base(database)
        {
            this.expression = expression;
            var parser = new LinqParser<M, Delete<M>>(this);
            parser.ParseTree(expression);
        }

        public override Tuple<string, IEnumerable<IDataParameter>> ToSql()
        {
            var where = GetWhere();

            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Format("DELETE FROM {0} \r\n{1}", Modl<M>.Table, where.Item1),
                where.Item2);
        }

        //public override string ToString()
        //{
        //    return string.Format("DELETE FROM {0} \r\nWHERE \r\n{1}", Modl<C>.TableName, QueryPartsToString());
        //}
    }
}
