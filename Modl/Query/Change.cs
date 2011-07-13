using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.DatabaseProviders;

namespace Modl
{
    public abstract class Change<C> : Query<C, Change<C>> where C : Modl<C>, new()
    {
        public Change(Database database) : base(database) { }

        protected Dictionary<string, object> ChangeValues = new Dictionary<string, object>();

        public Change<C> With(string key, string value)
        {
            return With<string>(key, value);
        }

        public Change<C> With<V>(string key, V value)
        {
            ChangeValues.Add(key, value);
            return this;
        }
    }
}
