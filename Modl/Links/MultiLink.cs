using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modl.Instance;

namespace Modl
{
    public class MultiLink<M> : BaseLink<M>, IEnumerable<M>
        where M : IModl, new()
    {
        internal MultiLink(string name, IModl m) : base(name, m)
        {
        }

        public void Add(M m)
        {
            Backer.GetRelation(Name).Add(m.Id());
        }

        public IEnumerator<M> GetEnumerator() =>
            Backer
            .GetRelation(Name)
            .All()
            .Select(id => Modl<M>.Get(id))
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}