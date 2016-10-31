using System.Collections.Generic;
using Modl.Exceptions;
using Modl.Structure.Storage;

namespace Modl.Instance
{
    public class InstanceStore<M>
        where M : IModl, new()
    {
        public static InstanceStore<M> ForThisModl { get; } = new InstanceStore<M>();
        private Dictionary<Identity, UniqueInstancesCollection<M>> LifetimeInstances { get; } = new Dictionary<Identity, UniqueInstancesCollection<M>>();

        public UniqueInstancesCollection<M> AddInstanceFromStorage(Identity id, IEnumerable<Container> storage)
        {
            if (HasCollection(id))
                throw new InvalidIdException($"There is already a collection with id '{id}'");

            var collection = UniqueInstancesCollection<M>.FromStorage(id, storage);
            LifetimeInstances.Add(id, collection);

            return collection;
        }

        public UniqueInstancesCollection<M> AddNewInstance(Identity id, M modl)
        {
            if (HasCollection(id))
                throw new InvalidIdException($"There is already a collection with id '{id}'");

            var collection = UniqueInstancesCollection<M>.FromNew(id, modl);
            LifetimeInstances.Add(id, collection);

            return collection;
        }

        public void ChangeId(M modl, Identity newId)
        {
            if (HasCollection(newId))
                throw new InvalidIdException($"There is already a collection with id '{newId}'");

            var oldId = modl.Modl.Id;
            var collection = Get(oldId);

            collection.ChangeId(newId);
            LifetimeInstances.Remove(oldId);
            LifetimeInstances.Add(newId, collection);
        }

        public UniqueInstancesCollection<M> Get(Identity id)
        {
            if (HasCollection(id))
                return LifetimeInstances[id];

            throw new NotFoundException($"There is no collection with id '{id}'");
        }

        public M GetInstance(Identity id)
        {
            return Get(id).GetInstance();
        }

        public bool HasCollection(Identity id) => LifetimeInstances.ContainsKey(id);
    }
}