using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Metadata
{
    public class ModlProperty<M>
         where M : IModl, new()
    {
        public string Name { get; private set; }
        public string ModlName { get; private set; }
        public Type Type { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsForeignKey { get { return ForeignKeyType != null; } }
        public Type ForeignKeyType { get; private set; }
        private Func<M, object> Getter { get; set; }
        private Action<M, object> Setter { get; set; }

        public ModlProperty(PropertyInfo property, ModlLayer<M> layer)
        {
            Name = property.Name;
            ModlName = property.Name;
            Type = property.PropertyType;

            //string fieldName = property.Name;
            ////bool key = false;
            //Type foreignKey = null;

            foreach (var attribute in property.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    ModlName = ((NameAttribute)attribute).Name;
                else if (attribute is KeyAttribute)
                    IsPrimaryKey = true;
                else if (attribute is ForeignKeyAttribute)
                {
                    ForeignKeyType = ((ForeignKeyAttribute)attribute).Entity;

                    if (layer.HasParent && ForeignKeyType == layer.Parent.Type)
                        IsPrimaryKey = true;
                }
                //else if (attribute is CacheAttribute)
                //{
                //    Settings.CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                //    Settings.CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                //}
            }

            //content.SetValue(fieldName, Helper.GetDefault(property.PropertyType));

            Getter = (Func<M, object>)typeof(ModlProperty<M>)
                .GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(property.PropertyType)
                .Invoke(null, new object[] { property.GetGetMethod(true) });

            Setter = (Action<M, object>)typeof(ModlProperty<M>)
                .GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(property.PropertyType)
                .Invoke(null, new object[] { property.GetSetMethod(true) });

            //var modlProperty = new ModlProperty<M>
            //{
            //    Type = property.PropertyType,
            //    Name = property.Name,
            //    ModlName = fieldName,
            //    Getter = getDelegate,
            //    Setter = setDelegate
            //};

            //layer.Properties.Add(modlProperty.Name, modlProperty);

            //EmptyProperties.Add(new Tuple<PropertyInfo, Func<M, object>, Action<M, object>>(property, getDelegate, setDelegate));

            //SetFieldName(property.Name, fieldName);
            //SetFieldType(fieldName, property.PropertyType);


            //layer.Fields.Add(fieldName, property.PropertyType);

            //if (key)
            //    layer.Keys.Add(fieldName, property.PropertyType);

            //if (foreignKey != null)
            //    layer.ForeignKeys.Add(fieldName, foreignKey);
        }

        public object GetValue(M instance)
        {
            return Getter(instance);
        }

        public void SetValue(M instance, object value)
        {
            Setter(instance, value);
        }

        private static Func<M, object> MakeGetDelegate<T>(MethodInfo method)
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        private static Action<M, object> MakeSetDelegate<T>(MethodInfo method)
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, method);
            return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        }
    }
}
