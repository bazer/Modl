using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    //public enum IdType
    //{
    //    Int,
    //    Guid
    //}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface)]
    public class NameAttribute : Attribute
    {
        public string Name { get; private set; }
        //public IdType Type { get; private set; }

        public NameAttribute(string name)
        {
            Name = name;
            //Type = type;
        }
    }
}
