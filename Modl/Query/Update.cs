using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;
using System.Data;

namespace Modl.Query
{
    public class Update<C> : Change<C> where C : Modl<C>, new()
    {
        public Update(Database database) : base(database) { }

        protected Tuple<string, IEnumerable<IDataParameter>> GetWith()
        {
            int i = 0, j = 0;
            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Join(",", withList.Select(x => DatabaseProvider.GetParameterComparison(x.Key, Relation.Equal, "v" + i++))),
                withList.Select(x => DatabaseProvider.GetParameter("v" + j++, x.Value)));
        }

        public override Tuple<string, IEnumerable<IDataParameter>> ToSql()
        {
            var with = GetWith();
            var where = GetWhere();

            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Format("UPDATE {0} SET {1} \r\n{2}", Modl<C>.Table, with.Item1, where.Item1),
                with.Item2.Concat(where.Item2));
        }
    }
}
