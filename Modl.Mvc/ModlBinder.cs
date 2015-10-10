using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using Modl.Structure.Metadata;

namespace Modl.Mvc
{
    public class DbModlBinder<M> : DefaultModelBinder where M : IModl, new()
    {
        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type modelType)
        {

            var id = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + Modl<M>.Definitions.IdProperty.PropertyName);

            if (id == null)
                id = bindingContext.ValueProvider.GetValue(Modl<M>.Definitions.IdProperty.PropertyName);

            if (id != null)
                return Modl<M>.Get(id.AttemptedValue);

            return Modl<M>.New();

            //if (bindingContext.ModelType.GetInterface("IModl") != null && value != null)
            //{
            //    if (MvcHelper.IsNumeric(value.AttemptedValue))
            //    {
            //        var method = bindingContext.ModelType.GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).Single(x => x.Name == "Get" && x.GetParameters()[0].ParameterType == typeof(int));
            //        return method.Invoke(null, new object[] { Convert.ToInt32(value.AttemptedValue), true });
            //    }
            //}

            //return base.BindModel(controllerContext, bindingContext);
        }
    }
}
