using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Fields
{
    internal class Statics<C> where C : Modl<C>, new()
    {
        protected static Dictionary<string, string> Properties = new Dictionary<string, string>();

        internal static void SetFieldName(string propertyName, string fieldName)
        {
            Properties[propertyName] = fieldName;
        }

        internal static string GetFieldName(string propertyName)
        {
            return Properties[propertyName];
        }
    }
}
