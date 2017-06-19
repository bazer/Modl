using Modl.Instance;
using Modl.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using Modl.Exceptions;

namespace Modl
{
    public interface IChangeCollection : IEnumerable<IChange>
    {
        //IEnumerable<IChange> Changes { get; }

        ChangeCollection Concat(IEnumerable<IChange> modifications);
        ChangeCollection Concat(IMutable mutable);
        //ChangeCollection Concat(ChangeCollection mutation);
    }

    public struct ChangeCollection : IChangeCollection
    {
        private List<IChange> changes;
        //public bool IsCommited { get; private set; }

        public ChangeCollection(IEnumerable<IChange> changes)
        {
            //this.IsCommited = false;
            this.changes = changes.ToList();
        }

        //public IEnumerable<IChange> Changes => changes.AsEnumerable();

        public ChangeCollection Concat(IEnumerable<IChange> modifications)
        {
            //CheckIfMutable();

            return new ChangeCollection(this.changes.Concat(modifications));
        }

        public ChangeCollection Concat(IMutable mutable)
        {
            return Concat(mutable.GetChanges());
        }

        //public ChangeCollection Concat(ChangeCollection mutation)
        //{
        //    return Concat(mutation);
        //}

        public IEnumerator<IChange> GetEnumerator()
        {
            return changes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return changes.GetEnumerator();
        }

        public ICommit Commit(IUser user)
        {
            var commit = Handler.Commit(this, user);

            //IsCommited = true;

            return commit;
        }

        //private void CheckIfMutable()
        //{
        //    if (IsCommited)
        //        throw new NotMutableException("Changes have been commited, ChangeCollection is no longer mutable.");
        //}
    }
}