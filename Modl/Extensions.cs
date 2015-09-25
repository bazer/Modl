using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;
using Modl.Structure.Metadata;

namespace Modl
{
    public static class Extensions
    {
        internal static Backer GetBacker<M>(this M m) where M : IModl, new()
        {
            return Handler<M>.InitializeModl(m).ModlData.Backer;
        }

        public static M Modl<M>(this M m) where M : IModl, new()
        {
            return Handler<M>.InitializeModl(m);
        }

        public static bool IsNew<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsNew;
        }

        public static bool IsDeleted<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsDeleted;
        }

        public static bool IsModified<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsModified(m);
        }

        public static M SetId<M>(this M m, object value) where M : IModl, new()
        {
            m.GetBacker().SetId(value);
            return m;
        }

        public static object GetId<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().GetId();
        }

        public static T GetRelation<M, T>(this M m, string name) 
            where M: IModl, new()
            where T: IModl, new()
        {
            return m.GetBacker().GetRelationValue<T>(name);

            //if (id == null)
            //    return default(T);

            //return Handler<M>.Get<T>(id);
        }

        public static void SetRelation<M, T>(this M m, string name, T value)
            where M : IModl, new()
            where T : IModl, new()
        {
            m.GetBacker().SetRelationValue(name, value);
        }

        public static bool Save<M>(this M m) where M : IModl, new()
        {
            return Handler<M>.Save<M>(m);
        }

        public static bool Delete<M>(this M m) where M : IModl, new()
        {
            return Handler<M>.Delete<M>(m);
        }
    }
}
