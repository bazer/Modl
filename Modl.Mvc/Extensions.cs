/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Expressions;

namespace Modl.Mvc
{
    public static class Extensions
    {
        public static IEnumerable<SelectListItem> AsSelectList<M, IdType>(this IEnumerable<M> list, Func<M, string> text, Func<M, string> value = null) where M : Modl<M, IdType>, new()
        {
            if (value == null)
                value = x => x.Id.ToString();

            return from c in list
                   select new SelectListItem
                   {
                       Text = text.Invoke(c),
                       Value = value.Invoke(c)
                   };
        }
    }
}
