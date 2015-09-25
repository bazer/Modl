using Modl.Cache;
using Modl.Structure.Metadata;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl
{
    internal class Handler<M> where M : IModl, new()
    {
        public static Settings Settings { get { return Settings.Get(typeof(M)); } }
        public static Definitions Definitions { get { return Definitions.Get(typeof(M)); } }

        static Handler()
        {
        }
        
        internal static M InitializeModl<M>(M m) where M: IModl, new()
        {
            if (m.ModlData == null)
            {
                m.ModlData = new ModlData
                {
                    Backer = new Backer(typeof(M))
                };

                if (!Definitions.HasPrimaryKey)
                    m.ModlData.Backer.SetId(Guid.NewGuid());
            }

            return m;
        }

        internal static M AddFromStorage<M>(IEnumerable<Container> storage) where M : IModl, new()
        {
            var m = New<M>(storage.First().About.Id);
            m.ModlData.Backer.SetValuesFromStorage(storage);

            return m;
        }

        internal static M New<M>() where M : IModl, new()
        {
            return new M().Modl();
        }

        internal static M New<M>(object id) where M : IModl, new()
        {
            return New<M>().SetId(id);
        }

        internal static M Get<M>(object id) where M : IModl, new()
        {
            var m = AddFromStorage<M>(Materializer.Read(Definitions.Get(typeof(M)).GetIdentities(id), Settings.Get(typeof(M))).ToList());
            m.ModlData.Backer.IsNew = false;
            m.ModlData.Backer.ResetValuesToUnmodified();
            m.ModlData.Backer.WriteToInstance(m);

            return m;
        }

        internal static bool Save<M>(M m) where M : IModl, new()
        {
            var instance = m.ModlData.Backer;

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (!instance.IsModified(m))
                return false;

            Materializer.Write(instance.GetStorage(), Settings.Get(typeof(M)));

            instance.IsNew = false;
            instance.ResetValuesToUnmodified();

            return true;
        }

        internal static bool Delete<M>(M m) where M : IModl, new()
        {
            var instance = m.ModlData.Backer;

            if (instance.IsNew)
                throw new Exception(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            Materializer.Delete(instance.GetStorage(), Settings.Get(typeof(M)));
            instance.IsDeleted = true;

            return true;
        }
    }
}
