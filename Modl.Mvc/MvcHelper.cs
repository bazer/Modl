using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Mvc
{
    internal class MvcHelper
    {
        public static bool IsNumeric(string teststring)
        {
            int i;
            return (Int32.TryParse(teststring, out i));
        }
    }
}
