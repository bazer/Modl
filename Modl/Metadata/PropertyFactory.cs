using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Metadata
{
    public class PropertyFactory
    {
        public static Property Create(PropertyInfo propertyInfo, Type modlType)
        {
            var propertyType = propertyInfo.PropertyType;

            if (IsLink(propertyType))
                return new LinkProperty(propertyInfo, modlType);
            else
                return new Property(propertyInfo, modlType);
        }

        private static bool IsLink(Type propertyType) => 
            typeof(IModl).IsAssignableFrom(propertyType) ||
            (propertyType.IsGenericType &&
            (propertyType.GetGenericTypeDefinition() == typeof(ModlValue<>) ||
             propertyType.GetGenericTypeDefinition() == typeof(ModlCollection<>)));
    }
}
