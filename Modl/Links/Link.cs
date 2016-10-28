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

        public Identity Id => Relation.All().FirstOrDefault();

        public M Val
        {
            get
            {
                if (HasLinkValue)
                    return LinkValue;

                LinkValue = Handler<M>.Get(Id);
                return LinkValue;
            }
            set
            {
                LinkValue = value;
                Relation.Set(value.Id());
            }
        }

        private RelationValue Relation => Backer.GetRelation(Name);

        private bool HasLinkValue => LinkValue != null;
        private M LinkValue { get; set; }
    }
}