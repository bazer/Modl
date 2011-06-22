using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;

namespace Modl
{
    public class ModlBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (bindingContext.ModelType.GetInterface("IModl") != null && value != null)
            {
                if (Helper.IsNumeric(value.AttemptedValue))
                {
                    var method = bindingContext.ModelType.GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).Single(x => x.Name == "Get" && x.GetParameters()[0].ParameterType == typeof(int));
                    return method.Invoke(null, new object[] { Convert.ToInt32(value.AttemptedValue), true });
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
