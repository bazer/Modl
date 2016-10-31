using System.Collections.Generic;
using System.Linq;
using Modl.Exceptions;
using Modl.Helpers;
using Modl.Metadata;
using Modl.Structure.Storage;

namespace Modl.Instance
{
    public class UniqueInstancesCollection<M>
            where M : IModl, new()
    {
        internal UniqueInstancesCollection(Identity id)
        {
            this.Id = id;
            this.Backer = new Backer(typeof(M));
        }

        private Definitions Definitions => Handler<M>.Definitions;
        private Backer Backer { get; }
        private Identity Id { get; set; }
        private List<M> Instances { get; } = new List<M>();

        internal static UniqueInstancesCollection<M> FromNew(Identity id, M modl)
        {
            var collection = new UniqueInstancesCollection<M>(id);
            collection.ReadFromInstance(modl);
            collection.AddInstance(modl);

            return collection;
        }

        internal static UniqueInstancesCollection<M> FromStorage(Identity id, IEnumerable<Container> storage)
        {
            var collection = new UniqueInstancesCollection<M>(id);
            collection.Backer.SetValuesFromStorage(storage);
            collection.Backer.ResetValuesToUnmodified();
            collection.Backer.IsNew = false;

            return collection;
        }

        internal void AddRelation(IModl to)
        {
            foreach (LinkProperty property in Definitions.Properties.Where(x => x.IsLink && to.GetType() == (x as LinkProperty).LinkedModlType))
                Backer.GetRelation(property.PropertyName).Add(to.Modl.Id);

            WriteRelationsToAllInstances();
        }

        internal void ChangeId(Identity newId)
        {
            if (!Backer.IsNew)
                throw new InvalidIdException("Can't change id of a Modl that is not new");

            if (!IdConverter.HasValue(newId.Get()))
                throw new InvalidIdException("Can't change to an empty id");

            this.Id = newId;

            WriteNewModlDataToAllInstances();
            WriteIdToAllInstances();
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

        internal M GetInstance()
        {
            if (Backer.IsDeleted)
                throw new NotFoundException(string.Format("Trying to get a deleted object. Class: {0}, Id: {1}", Backer.ModlType, Id));

            var modl = new M();

            WriteToInstance(modl);
            AddInstance(modl);

            return modl;
        }

        internal bool Save()
        {
            if (Backer.IsDeleted)
                throw new NotFoundException(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", Backer.ModlType, Id));

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

            if (!Backer.IsNew && !Backer.IsModified())
                return false;

            var settings = Settings.Get(Backer.ModlType);

            if (!Id.IsSet && !Id.IsInternal && !Id.IsAutomatic && !settings.Endpoint.CanGenerateIds)
            {
                throw new InvalidIdException($"Id not set. Class: {Backer.ModlType}");
            }

            Materializer.Write(Definitions.GetStorage(Id, Backer), settings);

            Backer.IsNew = false;
            Backer.ResetValuesToUnmodified();
            WriteToAllInstances();

            return true;
        }

        internal void Sync(M m)
        {
            ReadFromInstance(m);
            WriteToAllInstances();
        }

        private void AddInstance(M modl)
        {
            WriteIdToInstance(modl);
            WriteRelationsToInstance(modl);

            modl.Modl = new ModlData(Id, Backer);
            Instances.Add(modl);
        }

        private void ReadFromInstance(M m)
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
                Backer.SetValue(property.PropertyName, property.GetValue(m));
        }

        private void WriteIdToAllInstances()
        {
            foreach (var instance in Instances)
                WriteIdToInstance(instance);
        }

        private void WriteIdToInstance(M m)
        {
            if (Definitions.HasIdProperty && (Id.IsAutomatic || Id.IsSet))
                Definitions.IdProperty.SetValue(m, Id.Get());
        }

        private void WriteNewModlDataToAllInstances()
        {
            var data = new ModlData(Id, Backer);

            foreach (var instance in Instances)
                instance.Modl = data;
        }

        private void WriteRelationsToAllInstances()
        {
            foreach (var instance in Instances)
                WriteRelationsToInstance(instance);
        }

        private void WriteRelationsToInstance(M m)
        {
            foreach (LinkProperty property in Definitions.Properties.Where(x => x.IsLink))
                property.SetLinkValue(m);
        }

        private void WriteToAllInstances(string propertyName = null)
        {
            foreach (var instance in Instances)
                WriteToInstance(instance, propertyName);
        }

        private void WriteToInstance(M m, string propertyName = null)
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
                if (propertyName == null || property.PropertyName == propertyName)
                    property.SetValue(m, Backer.GetValue<object>(property.PropertyName));
        }

        //internal void ReadRelationsFromInstance(M m)
        //{
        //    foreach (var property in Definitions.Properties.Where(x => x.IsLink))
        //        Backer.Setre(property.PropertyName, property.GetValue(m));
        //}
    }
}