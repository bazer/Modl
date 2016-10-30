using Modl.Instance;

namespace Modl
{
    public class BaseCollection<M>
        where M : IModl, new()
    {
        internal BaseCollection(string name, IModl m)
        {
            this.Name = name;
            this.ModlInstance = m;
        }

        internal Backer Backer => ModlInstance.Modl.Backer;
        internal string Name { get; }
        protected IModl ModlInstance { get; }
        protected RelationValue Relation => Backer.GetRelation(Name);
    }
}