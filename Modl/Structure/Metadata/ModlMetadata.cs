using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Metadata
{
    public class ModlMetadata<M>
        where M : IModl, new()
    {
        public ModlLayer<M> FirstLayer { get; private set; }

        //public Dictionary<string, string> Fields { get; private set; }
        //public Dictionary<string, Type> Types { get; private set; }
        //public List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>> EmptyProperties { get; private set; }
        

        //public string IdName { get { return FirstLayer.PrimaryKey.Name; } }
        //public Type IdType { get { return FirstLayer.PrimaryKey.Type; } }

        public ModlProperty<M> PrimaryKey { get { return FirstLayer.PrimaryKey; } }
        public string ModlName { get { return FirstLayer.ModlName; } }

        public List<ModlProperty<M>> Properties { get { return FirstLayer.AllProperties; } }

        public ModlMetadata()
        {
            FirstLayer = new ModlLayer<M>(typeof(M));
            //Fields = new Dictionary<string, string>();
            //Types = new Dictionary<string, Type>();
            //EmptyProperties = new List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>>();

            //ParseClassHierarchy(typeof(M));

        }

        //internal void SetFieldName(string propertyName, string fieldName)
        //{
        //    Fields[propertyName] = fieldName;
        //}

        //public string GetFieldName(string propertyName)
        //{
        //    return Fields[propertyName];
        //}

        //internal void SetFieldType(string fieldName, Type type)
        //{
        //    Types[fieldName] = type;
        //}

        //public Type GetFieldType(string fieldName)
        //{
        //    return Types[fieldName];
        //}

        //private void ParseClassHierarchy(Type type)
        //{
        //    if (type.BaseType != null && type.BaseType != typeof(object))
        //        ParseClassHierarchy(type.BaseType);

        //    Layers.Insert(0, new ModlLayer<M>(type));
        //}
    }
}
