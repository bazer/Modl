using System;
using System.Collections.Generic;
using Modl.Exceptions;
using Modl.Structure.Storage;

namespace Modl.Instance
{
    public class InstanceStore<M>
        where M : IModl, new()
    {
        private static InstanceStore<M> StaticInstance;

        [ThreadStatic]
        private static InstanceStore<M> ThreadInstance;

        public static InstanceStore<M> ForThisModl
        {
            get
            {
                if (Settings.GlobalSettings.InstanceSeparation == InstanceSeparation.None)
                {
                    if (StaticInstance == null)
                        StaticInstance = new InstanceStore<M>();

                    return StaticInstance;
                }
                else if (Settings.GlobalSettings.InstanceSeparation == InstanceSeparation.Thread)
                {
                    if (ThreadInstance == null)
                        ThreadInstance = new InstanceStore<M>();

                    return ThreadInstance;
                }
                else
                {
                    return Settings.GlobalSettings.CustomInstanceSeparationDictionary()
                        .GetOrAdd(typeof(M), type => new InstanceStore<M>()) as InstanceStore<M>;
                }
            }
        }

        private Dictionary<Identity, UniqueInstancesCollection<M>> Collections { get; } = new Dictionary<Identity, UniqueInstancesCollection<M>>();

        public UniqueInstancesCollection<M> AddInstanceFromStorage(Identity id, IEnumerable<Container> storage)
        {
            if (HasCollection(id))
                throw new InvalidIdException($"There is already a collection with id '{id}'");

            var collection = UniqueInstancesCollection<M>.FromStorage(id, storage);
            Collections.Add(id, collection);

            return collection;
        }

        public UniqueInstancesCollection<M> AddNewInstance(Identity id, M modl)
        {
            if (HasCollection(id))
                throw new InvalidIdException($"There is already a collection with id '{id}'");

            var collection = UniqueInstancesCollection<M>.FromNew(id, modl);
            Collections.Add(id, collection);

            return collection;
        }

        public void ChangeId(M modl, Identity newId)
        {
            if (HasCollection(newId))
                throw new InvalidIdException($"There is already a collection with id '{newId}'");

            var oldId = modl.Modl.Id;
            var collection = Get(oldId);

            collection.ChangeId(newId);
            Collections.Remove(oldId);
            Collections.Add(newId, collection);
        }

        public UniqueInstancesCollection<M> Get(Identity id)
        {
            if (HasCollection(id))
                return Collections[id];

            throw new NotFoundException($"There is no collection with id '{id}'");
        }

        public M GetInstance(Identity id)
        {
            return Get(id).GetInstance();
        }

        public bool HasCollection(Identity id) => Collections.ContainsKey(id);
    }
}