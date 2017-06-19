using Modl.Instance;
using System;
using Modl.Repository;

namespace Modl
{
    public interface IModlData
    {
        Backer Backer { get; }
        ICommit Commit { get; }
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
        public ICommit Commit => throw new NotImplementedException();
        public Identity Id { get; }
    }
}