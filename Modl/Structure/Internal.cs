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
    internal class Internal//<M>
        //where M : IModl, new()
    {
        //public static Settings Settings { get; private set; }
        //public static Metadata Metadata { get; private set; }
        //private static Dictionary<string, InstanceData> Instances { get; set; }
        

        static Internal()
        {
            //Settings = new Settings();
            //Metadata = new Metadata();
            //Instances = new Dictionary<string, InstanceData>();
        }

        

        //internal static InstanceData GetInstance(IModl m)
        //{
        //    if (m == null)
        //        throw new NullReferenceException("Modl object is null");

        //    InstanceData content;
        //    if (!Instances.TryGetValue(m.ModlData.Id, out content))
        //        throw new Exception("The instance hasn't been attached");

        //    return content;
        //}

        //internal static void RemoveInstance(IModl m)
        //{
        //    if (m == null)
        //        throw new NullReferenceException("Modl object is null");

        //    Instances.Remove(m.ModlData.Id);
        //}

        ////internal static bool HasInstance(IModl m)
        ////{
        ////    if (string.IsNullOrWhiteSpace(m.ModlData.Id))
        ////        throw new Exception("The instance doesn't have a ModlId");

        ////    return Instances.ContainsKey(m.ModlData.Id);
        ////}

        //internal static void AddInstance<M>(M m) where M: IModl
        //{
        //    if (string.IsNullOrWhiteSpace(m.ModlData.Id))
        //        throw new Exception("The instance doesn't have a ModlId");

        //    if (m.ModlData.Instance == null)
        //        m.ModlData.Instance = new InstanceData(Metadata.Metadata.Get(m.GetType()));

        //    //if (!HasInstance(m))
        //    //    Instances.Add(m.ModlData.Id, new InstanceData(Metadata.Metadata.Get(m.GetType())));
        //}

        internal static M AddFromStorage<M>(IEnumerable<Storage.Storage> storage) where M : IModl, new()
        {
            var m = Modl<M>.New(storage.First().About.Id);
            m.ModlData.Instance.SetValuesFromStorage(storage);

            return m;

            //var instance = new M().Modl().GetInstance();
            //instance.SetValuesFromStorage(storage);

            //return instance;
        }

        internal static M Get<M>(string id) where M : IModl, new()
        {
            //if (Instances.ContainsKey(id))
            //    return Instances[id].ConnectedObject;

            var m = AddFromStorage<M>(Materializer.Read(Metadata.Metadata.Get(typeof(M)).GetIdentities(id), Settings.Get(typeof(M))).ToList());
            m.ModlData.Instance.IsNew = false;
            m.ModlData.Instance.ResetFields();
            m.ModlData.Instance.WriteToInstance(m);

            return m;

            //var modlInstance = AddFromStorage<M>(Materializer.Read(Metadata.Metadata.Get(typeof(M)).GetIdentities(id), Settings.Get(typeof(M))).ToList());
            //modlInstance.IsNew = false;
            //modlInstance.ResetFields();
            //modlInstance.WriteToInstance();

            //return (M)modlInstance.ModlObject;
        }


        internal static bool Save<M>(M m) where M : IModl, new()
        {
            var instance = m.GetInstance();

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (!instance.IsModified(m))
                return false;

            Materializer.Write(instance.GetStorage(), Settings.Get(typeof(M)));

            instance.IsNew = false;
            instance.ResetFields();

            return true;
        }

        internal static bool Delete<M>(M m) where M : IModl, new()
        {
            var instance = m.GetInstance();

            if (instance.IsNew)
                throw new Exception(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            Materializer.Delete(instance.GetStorage(), Settings.Get(typeof(M)));
            instance.IsDeleted = true;
            //m.RemoveInstance();

            return true;
        }
    }
}
