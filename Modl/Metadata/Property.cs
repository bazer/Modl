using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Metadata
{
    public class Property
    {
        public string PropertyName { get; private set; }
        public string StorageName { get; private set; }
        public Type ModlType { get; private set; }
        public Type PropertyType { get; private set; }
        public PropertyInfo PropertyInfo { get; set; }
        public bool IsPrimaryKey { get; private set; }
        public bool AutomaticKey { get; private set; }
        public bool IsRelation { get; set; }

        //public bool IsForeignKey { get { return ForeignKeyType != null; } }
        //public Type ForeignKeyType { get; private set; }

        public Property(PropertyInfo property, Layer layer)
        {
            PropertyInfo = property;
            PropertyName = property.Name;
            StorageName = property.Name;
            PropertyType = property.PropertyType;
            ModlType = layer.Type;

            //if (PropertyType is IModl)
            if (typeof(IModl).IsAssignableFrom(PropertyType))
                IsRelation = true;

            foreach (var attribute in property.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    StorageName = ((NameAttribute)attribute).Name;
                else if (attribute is KeyAttribute)
                {
                    IsPrimaryKey = true;
                    AutomaticKey = (attribute as KeyAttribute).Automatic;
                }
                //else if (attribute is ForeignKeyAttribute)
                //{
                //    ForeignKeyType = ((ForeignKeyAttribute)attribute).Entity;

                //    if (layer.HasParent && ForeignKeyType == layer.Parent.Type)
                //        IsPrimaryKey = true;
                //}
                //else if (attribute is CacheAttribute)
                //{
                //    Settings.CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                //    Settings.CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                //}
            }

            //Getter = (GetDelegate<IModl>)typeof(Property)
            //    .GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
            //    .MakeGenericMethod(layer.Type, property.PropertyType)
            //    .Invoke(null, new object[] { property.GetGetMethod(true) });

            //Setter = (SetDelegate<IModl, object>)typeof(Property)
            //    .GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
            //    .MakeGenericMethod(layer.Type, property.PropertyType)
            //    .Invoke(null, new object[] { property.GetSetMethod(true) });
        }

        object getter = null;
        public object GetValue<M>(M instance) where M : IModl
        {
            if (getter == null)
                getter = (Func<M, object>)typeof(Property)
                    .GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(ModlType, PropertyType)
                    .Invoke(null, new object[] { PropertyInfo.GetGetMethod(true) });

            return (getter as Func<M, object>)(instance);
        }

        object setter = null;
        public void SetValue<M>(M instance, object value) where M : IModl
        {
            if (setter == null)
                setter = (Action<M, object>)typeof(Property)
                    .GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(ModlType, PropertyType)
                    .Invoke(null, new object[] { PropertyInfo.GetSetMethod(true) });

            (setter as Action<M, object>)(instance, value);
        }

        private static Func<M, object> MakeGetDelegate<M, T>(MethodInfo method) where M : IModl
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        private static Action<M, object> MakeSetDelegate<M, T>(MethodInfo method) where M : IModl
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, method);
            return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        }
    }
}
