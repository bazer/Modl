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

        public string IdName { get { return Layers.First().PrimaryKeyName; } }
        public Type IdType { get { return Layers.First().PrimaryKeyType; } }
        public string ModlName { get { return Layers.First().Name; } }

        public ModlMetadata()
        {
            Layers = new List<ModlLayer>();
            Properties = new Dictionary<string, string>();
            Types = new Dictionary<string, Type>();
            EmptyProperties = new List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>>();

            ParseClassHierarchy(typeof(M));
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

        private void ParseClassHierarchy(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
                ParseClassHierarchy(type.BaseType);

            ModlLayer layer = new Structure.ModlLayer();
            layer.Name = type.Name;
            layer.Type = type;

            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    layer.Name = ((NameAttribute)attribute).Name;
                //else if (attribute is CacheAttribute)
                //{
                //    Settings.CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                //    Settings.CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                //}
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.CanWrite)
                {
                    string fieldName = property.Name;
                    bool key = false;
                    Type foreignKey = null;

                    foreach (var attribute in property.GetCustomAttributes(false))
                    {
                        if (attribute is NameAttribute)
                            fieldName = ((NameAttribute)attribute).Name;
                        else if (attribute is KeyAttribute)
                            key = true;
                        else if (attribute is ForeignKeyAttribute)
                            foreignKey = ((ForeignKeyAttribute)attribute).Entity;
                        //else if (attribute is CacheAttribute)
                        //{
                        //    Settings.CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                        //    Settings.CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                        //}
                    }

                    //content.SetValue(fieldName, Helper.GetDefault(property.PropertyType));

                    var getDelegate = (Func<M, object>)typeof(ModlMetadata<M>).GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetGetMethod(true) });
                    var setDelegate = (Action<M, object>)typeof(ModlMetadata<M>).GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetSetMethod(true) });
                    EmptyProperties.Add(new Tuple<PropertyInfo, Func<M, object>, Action<M, object>>(property, getDelegate, setDelegate));

                    SetFieldName(property.Name, fieldName);
                    SetFieldType(fieldName, property.PropertyType);


                    layer.Fields.Add(fieldName, property.PropertyType);

                    if (key)
                        layer.Keys.Add(fieldName, property.PropertyType);

                    if (foreignKey != null)
                        layer.ForeignKeys.Add(fieldName, foreignKey);
                }
            }

            Layers.Insert(0, layer);
        }

        private static Func<M, object> MakeGetDelegate<T>(MethodInfo method)
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        private static Action<M, object> MakeSetDelegate<T>(MethodInfo method)
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, method);
            return (m, t) => f(m, (T)t);
        }
    }
}
