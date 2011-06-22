using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Modl
{
    static class Extensions
    {

        public static List<SelectListItem> GetAsSelectList(this List<IModl> list)
        {
            List<SelectListItem> selectLister = new List<SelectListItem>();
            SelectListItem it;

            foreach (IModl o in list)
            {
                it = new SelectListItem();
                it.Text = o.Id.ToString();
                it.Value = o.Id.ToString();

                selectLister.Add(it);
            }

            return selectLister;
        }
    }
}
