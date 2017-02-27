using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Modl.Structure.Storage;
using System.Reflection;
using Modl.Exceptions;
using Modl.Instance;
using Modl.Metadata;
using Modl.Helpers;

namespace Modl.Instance
{
    public class Backer
    {
        public bool IsNew { get; internal set; } = true;
        public bool IsDeleted { get; internal set; } = false;
        //public bool HasId { get; set; } = false;
        //public Guid? InternalId { get; private set; }
        //public Identity Id { get; private set; }
        //public Guid InternalId { get; private set; }
        //public SimpleValue IdValue { get; private set; }
        public ValueBacker<SimpleValue> SimpleValueBacker { get; } = new ValueBacker<SimpleValue>();
        public ValueBacker<RelationValue> RelationValueBacker { get; } = new ValueBacker<RelationValue>();
        public Definitions Definitions { get; set; }
        public Type ModlType { get; set; }

        

        public Backer(Type modlType)
        {
            ModlType = modlType;
            Definitions = Definitions.Get(modlType);
            SetDefaultValues();
            //Id = Identity.FromTemporaryId(Guid.NewGuid(), this);
        }

        public bool IsModified() => SimpleValueBacker.IsModified || RelationValueBacker.IsModified;

        public T GetValue<T>(string name)
        {
            if (SimpleValueBacker.HasValue(name))
                return (T)SimpleValueBacker.GetValue(name).Get();
            else
                throw new NotImplementedException();

        }

        public void SetValue<T>(string name, T value)
        {
            if (SimpleValueBacker.HasValue(name))
                SimpleValueBacker.GetValue(name).Set(value);
            else
                throw new NotImplementedException();
        }

        public void AddValue(Property property)
        {
            if (property.IsLink)
                RelationValueBacker.AddValue(property.PropertyName, new RelationValue());
            else
                SimpleValueBacker.AddValue(property.PropertyName, new SimpleValue(GetDefault(property.PropertyType)));
        }

        public RelationValue GetRelation(string name)
        {
            if (RelationValueBacker.HasValue(name))
                return RelationValueBacker.GetValue(name);
            else
                throw new NotImplementedException();
        }


        internal void ResetValuesToUnmodified()
        {
            SimpleValueBacker.ResetValuesToUnmodified();
            RelationValueBacker.ResetValuesToUnmodified();
        }


        //internal void SetId(object value, bool allowCasting = true)
        //{
        //    if (!IsNew)
        //        throw new InvalidIdException("Can't change id of a Modl that is not new");

        //    if (Definitions.HasIdProperty)
        //        IdValue = new SimpleValue(IdConverter.Convert(value, allowCasting, Definitions.IdProperty.PropertyType));
        //    else
        //        IdValue = new SimpleValue(IdConverter.ConvertToGuid(value));

        //    //this.Id = Identity.FromId(IdValue.Get(), this);
        //}


        //internal object GetId()
        //{
        //    return IdValue?.Get();
        //}

        //internal bool HasId()
        //{
        //    var id = GetId();

        //    if (id == null)
        //        return false;
        //    else if (id is Guid)
        //        return id != null && (Guid)id != Guid.Empty;
        //    else if (id is int)
        //        return (int)id != 0;
        //    else if (id is string)
        //        return !string.IsNullOrWhiteSpace(id as string);
        //    else if (id.GetType().IsValueType)
        //        return id != Activator.CreateInstance(id.GetType());
        //    else
        //        return true;
        //}

        //internal void GenerateInternalId()
        //{
        //    InternalId = Guid.NewGuid();
        //}

        //internal void GenerateId()
        //{
        //    if (Definitions.HasIdProperty && Definitions.IdProperty.PropertyType != typeof(Guid))
        //        throw new InvalidIdException("Only Id of type Guid is supported");

        //    SetId(Guid.NewGuid());
        //}

        //public IEnumerable<Container> GetStorage()
        //{
        //    return Definitions.GetStorage(this);
        //}

        internal void SetValuesFromStorage(IEnumerable<Container> storage)
        {
            Definitions.SetValuesFromStorage(this, storage);
        }


        internal void SetDefaultValues()
        {
            foreach (var property in Definitions.Properties)
                AddValue(property);
        }


        //internal void ReadFromInstance<M>(M m) where M : class, IModl
        //{
        //    foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
        //        SetValue(property.PropertyName, property.GetValue(m));

        //}

        //internal void ReadFromInstanceId<M>(M m) where M : class, IModl
        //{
        //    if (Definitions.HasIdProperty)
        //    {
        //        var oldValue = GetId();
        //        var newValue = Definitions.IdProperty.GetValue(m);

        //        if (!IdConverter.AreEqual(Definitions.IdProperty.PropertyType, newValue, oldValue))
        //            SetId(newValue);

        //        //if ((Definitions.IdProperty.PropertyType == typeof(Guid) && (Guid)newValue != (Guid)oldValue) ||
        //        //    (Definitions.IdProperty.PropertyType != typeof(Guid) && newValue != oldValue))
        //        //    SetId(newValue);
        //    }
        //}

        

        //internal void WriteToInstance<M>(M m, string propertyName = null) where M : class, IModl
        //{
        //    foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
        //        if (propertyName == null || property.PropertyName == propertyName)
        //            property.SetValue(m, GetValue<object>(property.PropertyName));
        //}

        //internal void WriteToInstanceId<M>(M m) where M : class, IModl
        //{
        //    if (Definitions.HasIdProperty)
        //        Definitions.IdProperty.SetValue(m, IdValue.Get());
        //}

        //internal void WriteRelationsToInstance<M>(M m) where M : class, IModl, new()
        //{
        //    foreach (LinkProperty property in Definitions.Properties.Where(x => x.IsLink))
        //        property.SetLinkValue(m);
        //}

        private object GetDefault(Type type)
        {
            if (!type.IsValueType)
                return null;
            else
                return Activator.CreateInstance(type);
        }

        //internal bool Save<M>(M m, bool includeRelations) where M : class, IModl, new()
        //{
        //    if (IsDeleted)
        //        throw new NotFoundException(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

        //    ReadFromInstanceId(m);
        //    ReadFromInstance(m);

        //    if (includeRelations)
        //    {
        //        foreach (var property in Definitions.Properties.Where(x => x.IsLink))
        //        {
        //            typeof(Backer)
        //                .GetMethod("SaveRelation", BindingFlags.Instance | BindingFlags.NonPublic)
        //                .MakeGenericMethod(property.PropertyType)
        //                .Invoke(this, new object[] { property });
        //        }
        //    }

        //    if (!HasId() && Definitions.HasAutomaticId)
        //        GenerateId();
        //    else if (!HasId())
        //        throw new InvalidIdException($"Id not set. Class: {ModlType}");


        //    if (!IsNew && !IsModified())
        //        return false;

        //    Materializer.Write(GetStorage(), Settings.Get(ModlType));

        //    IsNew = false;
        //    ResetValuesToUnmodified();
        //    WriteToInstance(m);

        //    return true;
        //}

        //internal bool Delete()
        //{
        //    if (IsNew)
        //        throw new NotFoundException(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", ModlType, GetId()));

        //    if (IsDeleted)
        //        throw new NotFoundException(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

        //    Materializer.Delete(GetStorage(), Settings.Get(ModlType));
        //    IsDeleted = true;

        //    return true;
        //}
    }
}
