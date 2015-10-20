using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modl.Linq;
using Modl.Structure;
using Modl.Structure.Metadata;

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

        public static Query<M> Query()
        {
            return new Query<M>();
        }
    }
}

