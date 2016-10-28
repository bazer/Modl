using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;
using Modl.Helpers;
using Modl.Metadata;
using Modl.Structure.Storage;

namespace Modl.Instance
{
    public class InstanceStore<M>
        where M : IModl, new()
    {
        public static InstanceStore<M> ForThisModl { get; } = new InstanceStore<M>();
        private Dictionary<Identity, UniqueInstancesCollection<M>> LifetimeInstances { get; } = new Dictionary<Identity, UniqueInstancesCollection<M>>();

        //public void Add(Identity id)
        //{
        //    if (!HasCollection(id))
        //        LifetimeInstances.Add(id, new UniqueInstancesCollection<M>(id));
        //}

        public UniqueInstancesCollection<M> AddNewInstance(Identity id, M modl)
        {
            if (HasCollection(id))
                throw new InvalidIdException($"There is already a collection with id '{id}'");

            var collection = UniqueInstancesCollection<M>.FromNew(id, modl);
            LifetimeInstances.Add(id, collection);

            return collection;
        }

        public UniqueInstancesCollection<M> AddInstanceFromStorage(Identity id, IEnumerable<Container> storage)
        {
            if (HasCollection(id))
                throw new InvalidIdException($"There is already a collection with id '{id}'");

            var collection = UniqueInstancesCollection<M>.FromStorage(id, storage);
            LifetimeInstances.Add(id, collection);

            return collection;
        }

        public M GetInstance(Identity id)
        {
            return Get(id).GetInstance();
        }

        public bool HasCollection(Identity id) => LifetimeInstances.ContainsKey(id);

        public UniqueInstancesCollection<M> Get(Identity id)
        {
            if (HasCollection(id))
                return LifetimeInstances[id];

            throw new NotFoundException($"There is no collection with id '{id}'");
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

        //private UniqueInstancesCollection<M> GetOrAdd(Identity id)
        //{
        //    if (HasCollection(id))
        //    {
        //        return LifetimeInstances[id];
        //    }
        //    else
        //    {
        //        var collection = new UniqueInstancesCollection<M>(id);
        //        LifetimeInstances.Add(id, collection);

        //        return collection;
        //    }
        //}
    }

    public class UniqueInstancesCollection<M>
            where M : IModl, new()
    {
        public Backer Backer { get; }
        public Identity Id { get; private set; }
        public List<M> Instances { get; } = new List<M>();

        public Definitions Definitions => Handler<M>.Definitions;

        public UniqueInstancesCollection(Identity id)
        {
            this.Id = id;
            this.Backer = new Backer(typeof(M));
        }

        public static UniqueInstancesCollection<M> FromNew(Identity id, M modl)
        {
            var collection = new UniqueInstancesCollection<M>(id);

            //if (!collection.Definitions.HasIdProperty || collection.Definitions.HasAutomaticId)
            //if (collection.Definitions.HasAutomaticId || (collection.Definitions.HasIdProperty && id.IsSet))
            //    collection.Backer.SetId(id.Get());

            collection.ReadFromInstance(modl);
            collection.AddInstance(modl);

            return collection;
        }

        public static UniqueInstancesCollection<M> FromStorage(Identity id, IEnumerable<Container> storage)
        {
            var collection = new UniqueInstancesCollection<M>(id);

            //collection.Backer.SetId(storage.First().About.Id);
            collection.Backer.SetValuesFromStorage(storage);
            collection.Backer.ResetValuesToUnmodified();
            collection.Backer.IsNew = false;

            return collection;
        }

        public void ChangeId(Identity newId)
        {
            if (!Backer.IsNew)
                throw new InvalidIdException("Can't change id of a Modl that is not new");

            if (!IdConverter.HasValue(newId.Get()))
                throw new InvalidIdException("Can't change to an empty id");

            this.Id = newId;
            //this.Backer.SetId(newId.Get());

            WriteNewModlDataToAllInstances();
            WriteIdToAllInstances();
        }

        private void AddInstance(M modl)
        {
            WriteIdToInstance(modl);
            WriteRelationsToInstance(modl);

            modl.Modl = new ModlData(Id, Backer);
            Instances.Add(modl);
        }

        public M GetInstance()
        {
            if (Backer.IsDeleted)
                throw new NotFoundException(string.Format("Trying to get a deleted object. Class: {0}, Id: {1}", Backer.ModlType, Id));

            var modl = new M();

            WriteToInstance(modl);
            AddInstance(modl);

            return modl;
        }

        internal void Sync(M m)
        {
            ReadFromInstance(m);
            WriteToAllInstances();
        }

        internal void ReadFromInstance(M m)
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
                Backer.SetValue(property.PropertyName, property.GetValue(m));
        }

        //internal void ReadRelationsFromInstance(M m)
        //{
        //    foreach (var property in Definitions.Properties.Where(x => x.IsLink))
        //        Backer.Setre(property.PropertyName, property.GetValue(m));
        //}

        //internal void ReadFromInstanceId(M m)
        //{
        //    if (Definitions.HasIdProperty)
        //    {
        //        var oldValue = Backer.GetId();
        //        var newValue = Definitions.IdProperty.GetValue(m);

        //        if (!IdConverter.AreEqual(Definitions.IdProperty.PropertyType, newValue, oldValue))
        //            Backer.SetId(newValue);
        //    }
        //}

        private void WriteNewModlDataToAllInstances()
        {
            var data = new ModlData(Id, Backer);

            foreach (var instance in Instances)
                instance.Modl = data;
        }

        internal void WriteToAllInstances(string propertyName = null)
        {
            foreach (var instance in Instances)
                WriteToInstance(instance, propertyName);
        }

        internal void WriteToInstance(M m, string propertyName = null)
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
                if (propertyName == null || property.PropertyName == propertyName)
                    property.SetValue(m, Backer.GetValue<object>(property.PropertyName));
        }

        internal void WriteIdToAllInstances()
        {
            foreach (var instance in Instances)
                WriteIdToInstance(instance);
        }

        internal void WriteIdToInstance(M m)
        {
            if (Definitions.HasIdProperty && (Id.IsAutomatic || Id.IsSet))
                Definitions.IdProperty.SetValue(m, Id.Get());
        }

        internal void WriteRelationsToInstance(M m)
        {
            foreach (LinkProperty property in Definitions.Properties.Where(x => x.IsLink))
                property.SetLinkValue(m);
        }

        internal bool Save()
        {
            if (Backer.IsDeleted)
                throw new NotFoundException(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", Backer.ModlType, Id));

            //ReadFromInstanceId(m);
            //ReadFromInstance(m);

            //if (includeRelations)
            //{
            //    foreach (var property in Definitions.Properties.Where(x => x.IsLink))
            //    {
            //        typeof(Backer)
            //            .GetMethod("SaveRelation", BindingFlags.Instance | BindingFlags.NonPublic)
            //            .MakeGenericMethod(property.PropertyType)
            //            .Invoke(this, new object[] { property });
            //    }
            //}

            //if (!Backer.HasId() && Definitions.HasAutomaticId)
            //    Backer.GenerateId();
            

            if (!Backer.IsNew && !Backer.IsModified())
                return false;

            var settings = Settings.Get(Backer.ModlType);

            if (!Id.IsSet && !Id.IsInternal && !Id.IsAutomatic && !settings.Endpoint.CanGenerateIds)
            {
                throw new InvalidIdException($"Id not set. Class: {Backer.ModlType}");
            }
                //if (Id.IsSet && !settings.Endpoint.CanGenerateIds)
                //throw new InvalidIdException($"Id not set. Class: {Backer.ModlType}");

            Materializer.Write(Definitions.GetStorage(Id, Backer), settings);

            Backer.IsNew = false;
            Backer.ResetValuesToUnmodified();
            WriteToAllInstances();

            return true;
        }

        internal bool Delete()
        {
            if (Backer.IsNew)
                throw new NotFoundException(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", Backer.ModlType, Id));

            if (Backer.IsDeleted)
                throw new NotFoundException(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", Backer.ModlType, Id));

            Materializer.Delete(Definitions.GetStorage(Id, Backer), Settings.Get(Backer.ModlType));
            Backer.IsDeleted = true;

            return true;
        }
    }
}
