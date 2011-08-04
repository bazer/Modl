using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;
using System.Data;

namespace Modl.Query
{
    public class Delete<C> : Query<C, Delete<C>> where C : Modl<C>, new()
    {
        public Delete(Database database) : base(database) { }

        public override Tuple<string, IEnumerable<IDataParameter>> ToSql()
        {
            var where = GetWhere();

            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Format("DELETE FROM {0} \r\n{1}", Modl<C>.Table, where.Item1),
                where.Item2);
        }

        //public override string ToString()
        //{
        //    return string.Format("DELETE FROM {0} \r\nWHERE \r\n{1}", Modl<C>.TableName, QueryPartsToString());
        //}
    }
}
