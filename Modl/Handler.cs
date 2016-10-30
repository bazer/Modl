using System.Collections.Generic;
using System.Linq;
using Modl.Helpers;
using Modl.Instance;
using Modl.Metadata;
using Modl.Structure.Storage;

namespace Modl
{
    internal class Handler<M> where M : IModl, new()
    {
        static Handler()
        {
        }

        public static Definitions Definitions => Definitions.Get(typeof(M));
        public static InstanceStore<M> InstanceStore => InstanceStore<M>.ForThisModl;
        public static Settings Settings => Settings.Get(typeof(M));

        internal static void AddRelation(M from, IModl to)
        {
            Sync(from);

            InstanceStore.Get(from.Modl.Id).AddRelation(to);
        }

        internal static void ChangeId(M modl, object newId)
        {
            Sync(modl);

            var id = Identity.FromId(newId, Definitions);

            if (modl.Modl.Id != id)
                InstanceStore.ChangeId(modl, id);
        }

        internal static void Delete(Identity id)
        {
            var collection = InstanceStore.Get(id);
            collection.Delete();
        }

        internal static M Get(Identity id)
        {
            if (InstanceStore.HasCollection(id))
                return InstanceStore.GetInstance(id);
            else
                return InstanceStore.AddInstanceFromStorage(id, Materializer.Read(Definitions.GetIdentities(id), Settings).ToList()).GetInstance();
        }

        internal static IEnumerable<object> List()
        {
            return Materializer.List(Definitions.GetIdentities("").First(), Settings);
        }

        internal static M New()
        {
            return new M().Modl();
        }

        internal static M New(object id)
        {
            return New().Id(id);
        }

        internal static void Save(M modl)
        {
            Sync(modl);

            var collection = InstanceStore.Get(modl.Modl.Id);
            collection.Save();
        }

        internal static void Sync(M modl)
        {
            if (modl.Modl == null)
                InstanceStore.AddNewInstance(GetId(modl), modl);

            InstanceStore.Get(modl.Modl.Id).Sync(modl);

            if (HasIdChanged(modl))
                InstanceStore.ChangeId(modl, Identity.FromId(Definitions.IdProperty.GetValue(modl), Definitions));
        }

        private static Identity GetId(M modl)
        {
            if (!Definitions.HasAutomaticId && Definitions.HasIdProperty)
            {
                var id = Definitions.IdProperty.GetValue(modl);
                if (IdConverter.HasValue(id))
                    return Identity.FromId(id, Definitions);
            }

            return Identity.GenerateNewId(Definitions);
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
    }
}