using System;
using System.Collections.Generic;
using System.Linq;
using Modl.Metadata;

namespace Modl
{
    public interface IModl
    {
        IModlData Modl { get; set; }
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

        public static IEnumerable<M> GetAll()
        {
            return Handler<M>.List().Select(id => Get(id));
        }

        public static IEnumerable<object> List()
        {
            return Handler<M>.List();
        }

        public static IEnumerable<T> List<T>()
        {
            return Handler<M>.List().Select(id => (T)id);
        }

        public static IQueryable<M> Query()
        {
            throw new NotImplementedException();

            //var queryParser = QueryParser.CreateDefault();
            //return new Query<M>(queryParser, new QueryExecutor());
        }
    }
}

