using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public static class ModlExtensions
    {
        internal static ModlInstance<M> GetInstance<M>(this M m) where M : IModl, new()
        {
            return ModlInternal<M>.GetInstance(m);
        }

        public static M Modl<M>(this M m) where M : IModl, new()
        {
            ModlInternal<M>.AddInstance(m);
            return m;
        }

        public static bool IsNew<M>(this M m) where M : IModl, new()
        {
            return m.GetInstance().IsNew;
        }

        public static bool IsDeleted<M>(this M m) where M : IModl, new()
        {
            return m.GetInstance().IsDeleted;
        }

        public static bool IsModified<M>(this M m) where M : IModl, new()
        {
            return m.GetInstance().IsModified;
        }

        public static void SetId<M>(this M m, object value) where M : IModl, new()
        {
            m.GetInstance().SetId(value);
        }

        public static object GetId<M>(this M m) where M : IModl, new()
        {
            return m.GetInstance().GetId();
        }

        //public static ModlIdentity GetIdentity<M>(this M m) where M : IModl, new()
        //{
        //    return m.GetInstance().GetIdentity();
        //}

        //public static Dictionary<string, object> GetValues<M>(this M m) where M : IModl, new()
        //{
        //    return m.GetInstance().GetValues();
        //}

        public static bool Save<M>(this M m) where M : IModl, new()
        {
            return ModlInternal<M>.Save(m);
        }

        public static bool Delete<M>(this M m) where M : IModl, new()
        {
            return ModlInternal<M>.Delete(m);
        }
    }

}
