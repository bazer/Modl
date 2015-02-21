using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure
{
    public class ModlMetadata<M>
        where M : IModl, new()
    {
        public List<ModlLayer> Layers { get; private set; }

        public Dictionary<string, string> Properties { get; private set; }
        public Dictionary<string, Type> Types { get; private set; }
        public List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>> EmptyProperties { get; private set; }

        public string IdName { get { return Layers.Last().PrimaryKeyName; } }
        public Type IdType { get { return Layers.Last().PrimaryKeyType; } }
        public string ModlName { get { return Layers.Last().Name; } }

        public ModlMetadata()
        {
            Layers = new List<ModlLayer>();
            Properties = new Dictionary<string, string>();
            Types = new Dictionary<string, Type>();
            EmptyProperties = new List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>>();
        }

        internal void SetFieldName(string propertyName, string fieldName)
        {
            Properties[propertyName] = fieldName;
        }

        public string GetFieldName(string propertyName)
        {
            return Properties[propertyName];
        }

        internal void SetFieldType(string fieldName, Type type)
        {
            Types[fieldName] = type;
        }

        public Type GetFieldType(string fieldName)
        {
            return Types[fieldName];
        }
    }
}
