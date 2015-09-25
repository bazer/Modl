using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Metadata
{
    public class Relation
    {
        public string Name { get; private set; }
        public string ModlName { get; private set; }
        public Type ModlType { get; private set; }
        public PropertyInfo PropertyInfo { get; set; }

        public Relation(PropertyInfo property, Layer layer)
        {
            PropertyInfo = property;
            ModlType = layer.Type;
        }
    }
}
