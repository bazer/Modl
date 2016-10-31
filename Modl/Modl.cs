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
        static Modl()
        {
        }

        public static Definitions Definitions => Handler<M>.Definitions;
        public static Settings Settings => Handler<M>.Settings;

        public static bool Exists(object id) => 
            List().Any(x => x.Equals(id));

        public static M Get(object id) => 
            Handler<M>.Get(id is Identity ? id as Identity : Identity.FromId(id, Definitions));

        public static IEnumerable<M> GetAll() => 
            Handler<M>.List().Select(id => Get(id));

        public static IEnumerable<object> List() => 
            Handler<M>.List();

        public static IEnumerable<T> List<T>() => 
            Handler<M>.List().Select(id => (T)id);

        public static M New() => 
            Handler<M>.New();

        public static M New(object id) => 
            Handler<M>.New(id);

        public static IQueryable<M> Query()
        {
            throw new NotImplementedException();

            //var queryParser = QueryParser.CreateDefault();
            //return new Query<M>(queryParser, new QueryExecutor());
        }
    }
}