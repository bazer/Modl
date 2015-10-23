using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modl.Linq;
using Modl.Structure;
using Modl.Structure.Metadata;
using Remotion.Linq.Parsing.Structure;

namespace Modl
{
    public interface IModl
    {
        IModlData Modl { get; set; }
    }

    public class Modl
    {
        public static object New(Type type)
        {
            return typeof(Modl<>)
                .MakeGenericType(type)
                .GetMethod("New", new Type[] { })
                .Invoke(null, new object[] { });
        }

        public static object Get(Type type, object id)
        {
            return typeof(Modl<>)
                .MakeGenericType(type)
                .GetMethod("Get", new Type[] { typeof(object) })
                .Invoke(null, new object[] { id });
        }

        public static IEnumerable<object> List(Type type)
        {
            return (IEnumerable<object>)typeof(Modl<>)
                .MakeGenericType(type)
                .GetMethod("List", new Type[] { })
                .Invoke(null, new object[] { });
        }
    }

    public class Modl<M> where M : IModl, new()
    {
        public static Settings Settings { get { return Handler<M>.Settings; } }
        public static Definitions Definitions { get { return Handler<M>.Definitions; } }

        static Modl()
        {
        }

        public static M New()
        {
            return Handler<M>.New();
        }

        public static M New(object id)
        {
            return Handler<M>.New(id);
        }

        public static M Get(object id)
        {
            return Handler<M>.Get(id);
        }

        public static IEnumerable<object> List()
        {
            return Handler<M>.List();
        }

        public static Query<M> Query()
        {
            var queryParser = QueryParser.CreateDefault();

            return new Query<M>(queryParser, new QueryExecutor());
        }
    }
}

