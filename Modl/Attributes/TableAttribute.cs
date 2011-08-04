using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; private set; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
