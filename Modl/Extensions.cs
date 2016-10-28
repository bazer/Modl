using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Instance;

namespace Modl
{
    public static class Extensions
    {
        public static M Delete<M>(this M m) where M : IModl, new()
        {
            Handler<M>.Delete(m.Id());
            return m;
        }

        public static Identity Id<M>(this M m) where M : IModl, new()
        {
            return m.Modl().Modl.Id;
        }

        public static M Id<M>(this M m, object value) where M : IModl, new()
        {
            Handler<M>.ChangeId(m, value);
            return m;
        }

        public static bool IsDeleted<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsDeleted;
        }

        public static bool IsInitialized<M>(this M m) where M : IModl, new() => m.Modl != null;

        public static bool IsModified<M>(this M m) where M : IModl, new()
        {
            var backer = m.GetBacker();
            Handler<M>.Sync(m);

            return backer.IsModified();
        }

        public static bool IsNew<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsNew;
        }
        public static M Modl<M>(this M m) where M : IModl, new()
        {
            Handler<M>.Sync(m);
            return m;
        }

        public static M Save<M>(this M m) where M : IModl, new()
        {
            Handler<M>.Save(m);
            return m;
        }

        internal static Backer GetBacker<M>(this M m) where M : IModl, new()
        {
            Handler<M>.Sync(m);
            return m.Modl.Backer;
        }
    }
}
