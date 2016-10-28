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
using Modl.Exceptions;
using Modl.Helpers;

namespace Modl
{
    internal class Handler<M> where M : IModl, new()
    {
        public static Settings Settings => Settings.Get(typeof(M));
        public static Definitions Definitions => Definitions.Get(typeof(M));
        public static InstanceStore<M> InstanceStore => InstanceStore<M>.ForThisModl;

        static Handler()
        {
        }
        
        //internal static M InitializeModl(M m)
        //{
        //    if (m.Modl == null)
        //        InstanceStore.AddNewInstance(GetId(m), m);

        //    return m;
        //}

        private static Identity GetId(M m)
        {
            if (!Definitions.HasAutomaticId && Definitions.HasIdProperty)
            {
                var id = Definitions.IdProperty.GetValue(m);
                if (IdConverter.HasValue(id))
                    return Identity.FromId(id, Definitions);
            }
                
            return Identity.GenerateNewId(Definitions);
        }


        internal static void ChangeId(M modl, object newId)
        {
            Sync(modl);

            var id = Identity.FromId(newId, Definitions);

            if (modl.Modl.Id != id)
                InstanceStore.ChangeId(modl, id);
        }

        //internal static M AddFromStorage(IEnumerable<Container> storage)
        //{
        //    var m = New();

        //    var idValue = storage.First().About.Id;
        //    var backer = m.Modl.Backer;
        //    backer.SetId(idValue);
        //    backer.WriteToInstanceId(m);
        //    backer.SetValuesFromStorage(storage);
        //    backer.ResetValuesToUnmodified();
        //    backer.WriteToInstance(m);
        //    backer.IsNew = false;

        //    return m;
        //}

        internal static M New()
        {
            return new M().Modl();
        }

        internal static M New(object id)
        {
            return New().Id(id);
        }

        internal static M Get(Identity id)
        {
            if (InstanceStore.HasCollection(id))
                return InstanceStore.GetInstance(id);
            else
                return InstanceStore.AddInstanceFromStorage(id, Materializer.Read(Definitions.GetIdentities(id), Settings).ToList()).GetInstance();

            //return AddFromStorage(Materializer.Read(Definitions.GetIdentities(id), Settings).ToList());
        }

        internal static void Sync(M modl)
        {
            if (modl.Modl == null)
                InstanceStore.AddNewInstance(GetId(modl), modl);

            InstanceStore.Get(modl.Modl.Id).Sync(modl);

            if (HasIdChanged(modl))
                InstanceStore.ChangeId(modl, Identity.FromId(Definitions.IdProperty.GetValue(modl), Definitions));
            //ChangeId(modl, Definitions.IdProperty.GetValue(modl));
        }

        private static bool HasIdChanged(M modl)
        {
            if (!Definitions.HasAutomaticId && Definitions.HasIdProperty)
            {
                var id = Definitions.IdProperty.GetValue(modl);
                return IdConverter.HasValue(id) && modl.Modl.Id != id;
            }
            else
                return false;
        }

        internal static void Save(M modl)
        {
            Sync(modl);

            var collection = InstanceStore.Get(modl.Modl.Id);
            collection.Save();
        }

        internal static void Delete(Identity id)
        {
            var collection = InstanceStore.Get(id);
            collection.Delete();
        }

        internal static IEnumerable<object> List()
        {
            return Materializer.List(Definitions.GetIdentities("").First(), Settings);
        }
    }
}
