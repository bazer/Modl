using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Attributes
{
    public enum IdType
    {
        Int,
        Guid
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class IdAttribute : Attribute
    {
        public string Name { get; private set; }
        public IdType Type { get; private set; }
        
        public IdAttribute(string name, IdType type = IdType.Int)
        {
            Name = name;
            Type = type;
        }
    }
}
