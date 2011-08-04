using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;

namespace Modl.Query
{
    public abstract class Change<M> : Query<M, Change<M>> 
        where M : Modl<M>, new()
    {
        public Change(Database database) : base(database) { }

        protected Dictionary<string, object> withList = new Dictionary<string, object>();

        public Change<M> With(string key, string value)
        {
            return With<string>(key, value);
        }

        public Change<M> With<V>(string key, V value)
        {
            withList.Add(key, value);
            return this;
        }
    }
}
