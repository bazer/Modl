using Modl.Instance;
using System;
using System.Collections.Generic;

namespace Modl
{
    public static class Extensions
    {
        public static M Delete<M>(this M m) where M : class, IModl
        {
            Handler<M>.Delete(m.Id());
            return m;
        }

        public static Identity Id<M>(this M m) where M : class, IModl =>
            m.Modl().Modl.Id;

        public static M Id<M>(this M m, object value) where M : class, IModl
        {
            Handler<M>.ChangeId(m, value);
            return m;
        }

        public static bool IsDeleted<M>(this M m) where M : class, IModl =>
            m.GetBacker().IsDeleted;

        //public static bool IsInitialized<M>(this M m) where M : class, IModl =>
        //    m.Modl != null;

        public static bool IsModified<M>(this M m) where M : class, IModl =>
            m.Modl().GetBacker().IsModified();

        public static bool IsNew<M>(this M m) where M : class, IModl =>
            m.GetBacker().IsNew;

        public static M Modl<M>(this M m) where M : class, IModl
        {
            //Handler<M>.Sync(m);
            return m;
        }

        public static M Save<M>(this M m) where M : class, IModl
        {
            //Handler<M>.Save(m);
            return m;
        }

        internal static Backer GetBacker<M>(this M m) where M : class, IModl =>
            m.Modl().Modl.Backer;

        public static T Validate<T>(this T m, Func<T, bool> e) where T : IMutable
        {
            return m;
        }

        public static ICommit Commit(this IEnumerable<IMutation> modifications, IUser who)
        {
            return Handler.Commit(new MutationCollection(modifications), who);
        }

        public static ICommit Commit(this IMutationCollection mutation, IUser who)
        {
            return Handler.Commit(mutation, who);
        }

        public static ICommit Commit(this IMutable mutable, IUser who)
        {
            return Handler.Commit(mutable.ToMutation(), who);
        }

        public static ICommit Push(this ICommit commit)
        {
            Handler.Push(commit);
            return commit;
        }
    }
}