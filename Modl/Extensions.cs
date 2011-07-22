using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Expressions;

namespace Modl
{
    public static class Extensions
    {
        public static IEnumerable<SelectListItem> AsSelectList<T>(this IEnumerable<T> list, Func<T, string> text, Func<T, string> value = null) where T : Modl<T>, new()
        {
            if (value == null)
                value = x => Helper.ConvertTo<int>(x.Id).ToString();

            return from c in list
                   select new SelectListItem
                   {
                       Text = text.Invoke(c),
                       Value = value.Invoke(c)
                   };
        }
    }
}
