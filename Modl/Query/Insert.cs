using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;
using System.Data;

namespace Modl.Query
{
    public class Insert<C> : Change<C> where C : Modl<C>, new()
    {
        public Insert(Database database) : base(database) { }

        protected Tuple<string, IEnumerable<IDataParameter>> GetWith()
        {
            int i = 0, j = 0;
            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Format("({0}) VALUES ({1})",
                    string.Join(",", withList.Keys),
                    string.Join(",", withList.Values.Select(x => DatabaseProvider.GetParameterValue("v" + i++)))),
                withList.Select(x => DatabaseProvider.GetParameter("v" + j++, x.Value)));

            //return string.Format("({0}) VALUES ({1})",
            //    string.Join(",", withList.Keys),
            //    string.Join(",", withList.Values.Select(x => "'" + x + "'"))
            //);
        }

        public override Tuple<string, IEnumerable<IDataParameter>> ToSql()
        {
            var with = GetWith();

            return new Tuple<string, IEnumerable<IDataParameter>>(
                string.Format("INSERT INTO {0} {1}", Modl<C>.TableName, with.Item1),
                with.Item2);
        }

        //public override string ToString()
        //{
        //    return string.Format("INSERT INTO {0} {1}", Modl<C>.TableName, ValuesToString());
        //}
    }
}
