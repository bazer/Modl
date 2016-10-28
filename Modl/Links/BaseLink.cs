using Modl.Instance;

namespace Modl
{
    public class BaseLink<M>
        where M : IModl, new()
    {
        internal BaseLink(string name, IModl m)
        {
            this.Name = name;
            this.ModlInstance = m;
        }

        internal Backer Backer => ModlInstance.Modl.Backer;
        internal string Name { get; }
        protected IModl ModlInstance { get; }
    }
}