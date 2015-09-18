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

namespace Modl.Structure
{
    internal class ModlInternal<M>
        where M : IModl, new()
    {
        public static ModlSettings Settings { get; private set; }
        public static ModlMetadata<M> Metadata { get; private set; }
        private static Dictionary<string, ModlInstance<M>> Instances { get; set; }

        static ModlInternal()
        {
            Settings = new ModlSettings();
            Metadata = new ModlMetadata<M>();
            Instances = new Dictionary<string, ModlInstance<M>>();
        }

        internal static ModlInstance<M> GetInstance(M m)
        {
            if (m == null)
                throw new NullReferenceException("Modl object is null");

            ModlInstance<M> content;
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
                Instances.Add(m.Id, new ModlInstance<M>(m));
        }

        internal static ModlInstance<M> AddFromStorage(IEnumerable<ModlStorage> storage)
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

            var modlInstance = ModlInternal<M>.AddFromStorage(ModlMaterializer.Read(Metadata.GetIdentities(id), Settings).ToList());
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

            ModlMaterializer.Write(instance.GetStorage(), Settings);

            instance.IsNew = false;
            instance.ResetFields();

            return true;
        }
    }
}
