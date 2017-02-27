using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;

namespace Modl.Metadata
{
    public enum LinkType
    {
        SimpleLink,
        Link,
        MultiLink
    }

    public class LinkProperty : Property
    {
        public LinkType LinkType { get; }
        public Type LinkedModlType { get; }

        public LinkProperty(PropertyInfo property, Type modlType) : base(property, modlType)
        {
            this.IsLink = true;

            if (!PropertyType.IsGenericType && typeof(IModl).IsAssignableFrom(PropertyType))
                LinkType = LinkType.SimpleLink;
            else if (PropertyType.GetGenericTypeDefinition() == typeof(ModlValue<>))
                LinkType = LinkType.Link;
            else if (PropertyType.GetGenericTypeDefinition() == typeof(ModlCollection<>))
                LinkType = LinkType.MultiLink;
            else
                throw new InvalidLinkTypeException($"There is no link type for {PropertyType.GetGenericTypeDefinition()}");

            if (LinkType == LinkType.SimpleLink)
                LinkedModlType = PropertyType;
            else
                LinkedModlType = PropertyType.GetGenericArguments().First();
        }

        public void SetLinkValue<M>(M m) where M : class, IModl
        {
            if (LinkType == LinkType.SimpleLink)
                throw new NotImplementedException();

            var linkObject = Activator.CreateInstance(PropertyType, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { PropertyName, m }, null, null);

            this.SetValue(m, linkObject);
        }
    }
}
