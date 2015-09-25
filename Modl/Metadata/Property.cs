using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Metadata
{
    public class Property
        //where M : IModl, new()
    {
        public string Name { get; private set; }
        public string ModlName { get; private set; }
        public Type ModlType { get; private set; }
        public Type PropertyType { get; private set; }
        public PropertyInfo PropertyInfo { get; set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsForeignKey { get { return ForeignKeyType != null; } }
        public Type ForeignKeyType { get; private set; }
        //private GetDelegate<IModl> Getter { get; set; }
        //private Func<IModl, object> Getter { get; set; }
        //private SetDelegate<IModl, object> Setter { get; set; }
        //private Action<IModl, object> Setter { get; set; }

        public Property(PropertyInfo property, Layer layer)
        {
            PropertyInfo = property;
            Name = property.Name;
            ModlName = property.Name;
            PropertyType = property.PropertyType;
            ModlType = layer.Type;

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

            //Getter = (GetDelegate<IModl>)Delegate.CreateDelegate(typeof(GetDelegate<IModl>), null, property.GetGetMethod(true));

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
                setter = (Action<IModl, object>)typeof(Property)
                    .GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(ModlType, PropertyType)
                    .Invoke(null, new object[] { PropertyInfo.GetSetMethod(true) });

            (setter as Action<IModl, object>)(instance, value);
        }

        //delegate object GetDelegate<IModl>(IModl m);// where M : IModl;

        //private static GetDelegate<M> MakeGetDelegate<M>(MethodInfo method) where M: IModl
        //{
        //    var f = (GetDelegate<M>)Delegate.CreateDelegate(typeof(GetDelegate<M>), null, method);
        //    return m => f(m);
        //}

        //private static Func<M, object> MakeGetDelegate<M>(MethodInfo method) where M : IModl
        //{
        //    var f = (Func<M, object>)Delegate.CreateDelegate(typeof(Func<M, object>), null, method);
        //    return m => f(m);
        //}

        private static Func<M, object> MakeGetDelegate<M, T>(MethodInfo method) where M : IModl
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        //delegate void SetDelegate<IModl, T>(IModl m, T value);// where M : IModl;
        //private static SetDelegate<IModl, object> MakeSetDelegate<M, T>(MethodInfo method) where M : IModl
        //{
        //    var f = Delegate.CreateDelegate(typeof(SetDelegate<M, T>), null, method) as SetDelegate<IModl, object>;
        //    return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        //}

        private static Action<M, object> MakeSetDelegate<M, T>(MethodInfo method) where M : IModl
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<IModl, T>), null, method);
            return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        }
    }

    //public static class PropertyReader<M> where M : IModl
    //{
    //    public static Func<M, object> Getter { get; set; }
    //    public static Action<M, object> Setter { get; set; }



    //    private static Func<M, object> MakeGetDelegate<M, T>(MethodInfo method)
    //    {
    //        var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
    //        return m => f(m);
    //    }

    //    private static Action<IModl, object> MakeSetDelegate<T>(MethodInfo method)
    //    {
    //        var f = (Action<IModl, T>)Delegate.CreateDelegate(typeof(Action<IModl, T>), null, method);
    //        return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
    //    }
    //}
}
