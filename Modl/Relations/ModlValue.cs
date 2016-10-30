using System.Linq;

namespace Modl
{
    public class ModlValue<M> : BaseCollection<M>
        where M : IModl, new()
    {
        internal ModlValue(string name, IModl m) : base(name, m)
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
                Handler<M>.AddRelation(value, ModlInstance);
            }
        }

        private bool HasLinkValue => LinkValue != null;
        private M LinkValue { get; set; }
    }
}