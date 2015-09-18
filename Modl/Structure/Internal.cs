using Modl.Cache;
using Modl.Structure;
using Modl.Structure.Metadata;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl.Structure
{
    internal class Internal<M>
        where M : IModl, new()
    {
        public static Settings Settings { get; private set; }
        public static Metadata<M> Metadata { get; private set; }
        private static Dictionary<string, Instance<M>> Instances { get; set; }

        static Internal()
        {
            Settings = new Settings();
            Metadata = new Metadata<M>();
            Instances = new Dictionary<string, Instance<M>>();
        }

        internal static Instance<M> GetInstance(M m)
        {
            if (m == null)
                throw new NullReferenceException("Modl object is null");

            Instance<M> content;
            if (!Instances.TryGetValue(m.Id, out content))
                throw new Exception("The instance hasn't been attached");

            return content;
        }

        internal static bool HasInstance(M m)
        {
            if (string.IsNullOrWhiteSpace(m.Id))
                throw new Exception("The instance doesn't have a ModlId");

            return Instances.ContainsKey(m.Id);
        }

        internal static void AddInstance(M m)
        {
            if (string.IsNullOrWhiteSpace(m.Id))
                throw new Exception("The instance doesn't have a ModlId");

            if (!HasInstance(m))
                Instances.Add(m.Id, new Instance<M>(m));
        }

        internal static Instance<M> AddFromStorage(IEnumerable<Storage.Storage> storage)
        {
            var instance = new M().Modl().GetInstance();
            instance.SetValuesFromStorage(storage);

            return instance;
        }

        internal static bool Delete(M m)
        {
            throw new NotImplementedException();
        }

        internal static M Get(string id)
        {
            if (Instances.ContainsKey(id))
                return Instances[id].Instance;

            var modlInstance = Internal<M>.AddFromStorage(Materializer.Read(Metadata.GetIdentities(id), Settings).ToList());
            modlInstance.IsNew = false;
            modlInstance.ResetFields();
            modlInstance.WriteToInstance();

            return modlInstance.Instance;
        }


        internal static bool Save(M m)
        {
            var instance = m.GetInstance();

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.Id));

            if (!instance.IsModified)
                return false;

            Materializer.Write(instance.GetStorage(), Settings);

            instance.IsNew = false;
            instance.ResetFields();

            return true;
        }
    }
}
