using Modl.Instance;

namespace Modl
{
    public interface IModlData
    {
        Backer Backer { get; }
        Identity Id { get; }
    }

    public class ModlData : IModlData
    {
        internal ModlData(Identity id, Backer backer)
        {
            this.Id = id;
            this.Backer = backer;
        }

        public Backer Backer { get; }
        public Identity Id { get; }
    }
}