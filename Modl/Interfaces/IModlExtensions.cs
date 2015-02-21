using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public static class IModlExtensions
    {
        internal static ModlData GetContent(this IModl instance)
        {
            if (instance == null)
                throw new NullReferenceException("Modl object is null");

            var content = ModlData.GetContents(instance);

            if (content == null)
                throw new Exception("The instance hasn't been attached");




            //if (content == null)
            //    content = AddInstance(instance);

            return content;
        }

        //internal static Content GetContent<M>(this M m) where M : IModl, new()
        //{
        //    if (m == null)
        //        throw new NullReferenceException("Modl object is null");

        //    return Statics<M>.GetContents(m);
        //    //throw new NotImplementedException();
        //}

        public static M Modl<M>(this M m) where M : IModl, new()
        {
            var content = ModlInternal<M>.AddInstance(m);
            return m;
        }

        public static bool IsNew(this IModl m)// where M : IModl, new()
        {
            return m.GetContent().IsNew;
        }

        public static bool IsDeleted(this IModl m)// where M : IModl, new()
        {
            return m.GetContent().IsDeleted;
        }

        public static bool IsModified<M>(this M m) where M : IModl, new()
        {
            return ModlInternal<M>.IsModified(m);
        }

        public static void SetId<M>(this M m, object value) where M : IModl, new()
        {
            ModlInternal<M>.SetId(m, value);
        }

        public static object GetId<M>(this M m) where M : IModl, new()
        {
            return ModlInternal<M>.GetId(m);
        }

        public static bool Save<M>(this M m) where M : IModl, new()
        {
            return ModlInternal<M>.Save(m);
        }
    }

}
