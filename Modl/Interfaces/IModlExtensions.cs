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
            if (string.IsNullOrWhiteSpace(m.Id))
                m.Id = Guid.NewGuid().ToString();

            ModlInternal<M>.AddInstance(m);
            return m;
        }

        //internal static M Modl<M>(this M m, string modlId) where M : IModl, new()
        //{
        //    m.ModlId = modlId;
        //    ModlInternal<M>.AddInstance(m);
        //    return m;
        //}

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

        //public static M SetId<M>(this M m, object value) where M : IModl, new()
        //{
        //    m.GetInstance().SetId(value);
        //    return m;
        //}

        public static object GetId<M>(this M m) where M : IModl, new()
        {
            return m.GetInstance().GetId();
        }

        public static T GetRelation<M, T>(this M m, string name) 
            where M: IModl, new()
            where T: IModl, new()
        {
            var id = m.GetInstance().GetValue<string>(name);

            if (id == null)
                return default(T);

            return ModlInternal<T>.Get(id);
        }

        public static void SetRelation<M, T>(this M m, string name, T value)
            where M : IModl, new()
            where T : IModl, new()
        {
            m.GetInstance().SetValue(name, value.Id);
            //var t = ModlInternal<T>.Get(id);

            //return t;
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
