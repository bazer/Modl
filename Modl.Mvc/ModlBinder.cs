/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

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
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;

namespace Modl.Mvc
{
    public class ModlBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (bindingContext.ModelType.GetInterface("IModl") != null && value != null)
            {
                if (MvcHelper.IsNumeric(value.AttemptedValue))
                {
                    var method = bindingContext.ModelType.GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).Single(x => x.Name == "Get" && x.GetParameters()[0].ParameterType == typeof(int));
                    return method.Invoke(null, new object[] { Convert.ToInt32(value.AttemptedValue), true });
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
