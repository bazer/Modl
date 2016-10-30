using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Modl
{
    public class ModlCollection<M> : BaseCollection<M>, IEnumerable<M>
        where M : IModl, new()
    {
        internal ModlCollection(string name, IModl m) : base(name, m)
        {
        }

        private List<M> LinkCollection { get; set; }

        public void Add(M m)
        {
            GetCollection().Add(m);
            Relation.Add(m.Id());
            Handler<M>.AddRelation(m, ModlInstance);
        }

        public IEnumerator<M> GetEnumerator() => GetCollection().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private List<M> GetCollection()
        {
            if (LinkCollection == null)
                LinkCollection = Relation.All().Select(id => Modl<M>.Get(id)).ToList();

            return LinkCollection;
        }
    }
}