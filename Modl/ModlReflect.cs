using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Modl
{
    public class ModlReflect
    {
        public static object New(Type type)
        {
            return InvokeMethod<object>(type, "New");
        }

        public static object New(Type type, object id)
        {
            return InvokeMethod<object>(type, "New", id);
        }

        public static object Get(Type type, object id)
        {
            return InvokeMethod<object>(type, "Get", id);
        }

        public static IEnumerable<object> GetAll(Type type)
        {
            return InvokeMethod<IEnumerable<object>>(type, "GetAll");
        }

        public static IEnumerable<object> List(Type type)
        {
            return InvokeMethod<IEnumerable<object>>(type, "List");
        }

        public static IEnumerable<T> List<T>(Type type)
        {
            return InvokeGenericMethod<IEnumerable<T>>(type, "List", typeof(T));
        }

        private static T InvokeMethod<T>(Type type, string name, params object[] args)
        {
            return (T)typeof(Modl<>)
                .MakeGenericType(type)
                .GetMethods().Single(x => x.Name == name && !x.IsGenericMethod && x.GetParameters().Count() == args.Length)
                .Invoke(null, args);
        }

        private static T InvokeGenericMethod<T>(Type type, string name, Type genericType, params object[] args)
        {
            return (T)typeof(Modl<>)
                .MakeGenericType(type)
                .GetMethods().Single(x => x.Name == name && x.IsGenericMethod && x.GetParameters().Count() == args.Length)
                .MakeGenericMethod(genericType)
                .Invoke(null, args);
        }
    }
}

