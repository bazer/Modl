using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;

namespace Modl.Metadata
{
    public class Property
    {
        public string PropertyName { get; }
        public string StorageName { get; }
        public Type ModlType { get; }
        public Type PropertyType { get; }
        public PropertyInfo PropertyInfo { get; }
        public bool IsId { get; }
        public bool IsAutomaticId { get; }
        public bool IsLink { get; protected set; } = false;

        //public bool IsForeignKey { get { return ForeignKeyType != null; } }
        //public Type ForeignKeyType { get; private set; }

        public Property(PropertyInfo property, Type modlType)
        {
            PropertyInfo = property;
            PropertyName = property.Name;
            StorageName = property.Name;
            PropertyType = property.PropertyType;
            ModlType = modlType;


            foreach (var attribute in property.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    StorageName = ((NameAttribute)attribute).Name;
                else if (attribute is IdAttribute)
                {
                    IsId = true;
                    IsAutomaticId = (attribute as IdAttribute).Automatic;

                    if (IsAutomaticId && PropertyType != typeof(Guid))
                        throw new InvalidIdException("Automatic Ids must be of type Guid");
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
        }

        object getter = null;
        public object GetValue<M>(M m) where M : class, IModl
        {
            if (getter == null)
                getter = (Func<M, object>)typeof(Property)
                    .GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(ModlType, PropertyType)
                    .Invoke(null, new object[] { PropertyInfo.GetGetMethod(true) });

            return (getter as Func<M, object>)(m);
        }

        object setter = null;
        public void SetValue<M>(M m, object value) where M : class, IModl
        {
            if (setter == null)
                setter = (Action<M, object>)typeof(Property)
                    .GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(ModlType, PropertyType)
                    .Invoke(null, new object[] { PropertyInfo.GetSetMethod(true) });

            (setter as Action<M, object>)(m, value);
        }

        private static Func<M, object> MakeGetDelegate<M, T>(MethodInfo method) where M : class, IModl
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        private static Action<M, object> MakeSetDelegate<M, T>(MethodInfo method) where M : class, IModl
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, method);
            return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        }
    }
}
