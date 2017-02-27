using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Expressions;

namespace Modl.Mvc
{
    public static class Extensions
    {
        public static IEnumerable<SelectListItem> AsSelectList<M>(this IEnumerable<M> list, Func<M, string> text, Func<M, string> value = null) where M : class, IMvcModl
        {
            if (value == null)
                value = x => x.Id().ToString();

            return from c in list
                   select new SelectListItem
                   {
                       Text = text.Invoke(c),
                       Value = value.Invoke(c)
                   };
        }
    }
}
