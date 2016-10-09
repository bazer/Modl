using Modl.Instance;

namespace Modl
{
    public class BaseLink<M>
    {
        internal BaseLink(string name, IModl m)
        {
            this.Name = name;
            this.Backer = m.Modl.Backer;
        }

        internal Backer Backer { get; set; }
        internal string Name { get; set; }
    }
}