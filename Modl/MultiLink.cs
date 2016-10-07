using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Instance;

namespace Modl
{
    public class MultiLink<M> : IEnumerable<M>
        where M : IModl, new()
    {
        internal string Name { get; set; }
        internal Backer Backer { get; set; }

        internal MultiLink(string name, IModl m)
        {
            this.Name = name;
            this.Backer = m.Modl.Backer;
        }

        public IEnumerator<M> GetEnumerator()
        {
            return Backer
                .GetRelation(Name)
                .All()
                .Select(id => Modl<M>.Get(id))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(M m)
        {
            Backer.GetRelation(Name).Add(new RelationIdValue(m.Id().Get()));
        }
    }
}
