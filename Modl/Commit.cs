using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modl
{
    public interface ICommit
    {
        Guid Id { get; }
        IEnumerable<IMutation> Modifications { get; }
        IEnumerable<ICommit> Next { get; }
        IEnumerable<ICommit> Previous { get; }
        DateTime When { get; }
        IUser User { get; }
    }

    public class Commit : ICommit
    {
        private List<IMutation> modifications;
        private List<ICommit> next;
        private List<ICommit> previous;

        public Commit(IMutationCollection mutation, IUser who)
        {
            this.modifications = mutation.Modifications.ToList();
            this.When = DateTime.UtcNow;
            this.Id = Guid.NewGuid();
            this.User = who;
            this.next = new List<ICommit>();
            this.previous = new List<ICommit>();

            
        }

        

        public Guid Id { get; }

        public IEnumerable<IMutation> Modifications => modifications.AsEnumerable();
        public IEnumerable<ICommit> Next => next.AsEnumerable();
        public IEnumerable<ICommit> Previous => next.AsEnumerable();
        public DateTime When { get; }

        public IUser User { get; }
    }
}