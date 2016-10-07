using Modl.Cache;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Metadata;
using Modl.Instance;

namespace Modl
{
    internal class Handler<M> where M : IModl, new()
    {
        public static Settings Settings { get { return Settings.Get(typeof(M)); } }
        public static Definitions Definitions { get { return Definitions.Get(typeof(M)); } }

        static Handler()
        {
        }
        
        internal static M InitializeModl(M m)
        {
            if (m.Modl == null)
            {
                var backer = new Backer(typeof(M));
                backer.ReadFromInstanceId(m);
                backer.ReadFromInstance(m);

                if (Definitions.HasAutomaticId && !backer.HasId())
                {
                    backer.GenerateId();
                    backer.WriteToInstanceId(m);
                }

                m.Modl = new ModlData
                {
                    Backer = backer
                };

                backer.WriteRelationsToInstance(m);
            }

            return m;
        }

        internal static M AddFromStorage(IEnumerable<Container> storage)
        {
            var m = New();

            var idValue = storage.First().About.Id;
            var backer = m.Modl.Backer;
            backer.SetId(idValue);
            backer.WriteToInstanceId(m);
            backer.SetValuesFromStorage(storage);
            backer.ResetValuesToUnmodified();
            backer.WriteToInstance(m);
            backer.IsNew = false;

            return m;
        }

        internal static M New()
        {
            return new M().Modl();
        }

        internal static M New(object id)
        {
            return New().Id(id);
        }

        internal static M Get(object id)
        {
            return AddFromStorage(Materializer.Read(Definitions.GetIdentities(id), Settings).ToList());
        }

        internal static IEnumerable<object> List()
        {
            return Materializer.List(Definitions.GetIdentities("").First(), Settings);
        }
    }
}
