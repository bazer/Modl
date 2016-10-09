using System.Collections.Generic;
using System.Linq;
using Modl.Instance;

namespace Modl
{
    public class Link<M> : BaseLink<M>
        where M : IModl, new()
    {
        internal Link(string name, IModl m) : base(name, m)
        {
        }

        public object Id => Relation.All().FirstOrDefault();

        public M Val
        {
            get
            {
                return Modl<M>.Get(Id);
            }
            set
            {
                Relation.Set(new List<RelationIdValue> { new RelationIdValue(value) });
            }
        }

        private RelationValue Relation => Backer.GetRelation(Name);
    }
}