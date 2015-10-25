using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<object> List(Type type)
        {
            return InvokeMethod<IEnumerable<object>>(type, "List");
        }

        private static T InvokeMethod<T>(Type type, string name, params object[] args)
        {
            return (T)typeof(Modl<>)
                .MakeGenericType(type)
                .GetMethod(name, args.Select(x => x.GetType()).ToArray())
                .Invoke(null, args);
        }
    }
}

