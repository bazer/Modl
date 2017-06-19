using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modl
{
    public interface ICommit
    {
        Guid Id { get; }
        IEnumerable<IChange> Changes { get; }
        IEnumerable<ICommit> Next { get; }
        IEnumerable<ICommit> Previous { get; }
        DateTime When { get; }
        IUser User { get; }
    }

    public class Commit : ICommit
    {
        private List<IChange> changes;
        private List<ICommit> next;
        private List<ICommit> previous;

        public Commit(IChangeCollection changes, IUser user)
        {
            this.changes = changes.ToList();
            this.When = DateTime.UtcNow;
            this.Id = Guid.NewGuid();
            this.User = user;
            this.next = new List<ICommit>();
            this.previous = new List<ICommit>();
        }

        public Guid Id { get; }

        public IEnumerable<IChange> Changes => changes.AsEnumerable();
        public IEnumerable<ICommit> Next => next.AsEnumerable();
        public IEnumerable<ICommit> Previous => next.AsEnumerable();
        public DateTime When { get; }

        public IUser User { get; }
    }
}