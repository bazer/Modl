using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdAttribute : Attribute
    {
        public bool Automatic { get; set; }

        public IdAttribute(bool automatic = false)
        {
            this.Automatic = automatic;
        }
    }
}
