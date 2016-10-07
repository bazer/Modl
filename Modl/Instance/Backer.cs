using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Modl.Structure.Storage;
using System.Reflection;
using Modl.Exceptions;
using Modl.Instance;
using Modl.Metadata;

namespace Modl.Instance
{
    public class Backer
    {
        public bool IsNew { get; internal set; } = true;
        public bool IsDeleted { get; private set; } = false;
        //public bool HasId { get; set; } = false;
        //public Guid? InternalId { get; private set; }
        public SimpleValue IdValue { get; private set; }
        public ValueBacker<SimpleValue> SimpleValueBacker { get; } = new ValueBacker<SimpleValue>();
        public ValueBacker<RelationValue> RelationValueBacker { get; } = new ValueBacker<RelationValue>();
        public Definitions Definitions { get; set; }
        public Type ModlType { get; set; }

        public Backer(Type modlType)
        {
            ModlType = modlType;
            Definitions = Definitions.Get(modlType);
            SetDefaultValues();
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


        internal void SetId(object value, bool allowCasting = true)
        {
            if (!IsNew)
                throw new InvalidIdException("Can't change id of a Modl that is not new");

            if (Definitions.HasIdProperty)
            {
                var id = Definitions.IdProperty;

                if (id.PropertyType == typeof(Guid))
                    value = ConvertToGuid(value, allowCasting);

                if (id.PropertyType == typeof(int))
                    value = ConvertToInt32(value, allowCasting);

                if (id.PropertyType == typeof(string))
                    value = ConvertToString(value, allowCasting);

                if (id.PropertyType != value.GetType())
                    throw new InvalidIdException($"Id value should be of type {id.PropertyType}, but is of type {value.GetType()}");

                //IdValue.Set(value);
                //SetValue(id.PropertyName, value);
                IdValue = new SimpleValue(value);
                
            }
            else
            {
                //InternalId = ConvertToGuid(value);
                IdValue = new SimpleValue(ConvertToGuid(value));
            }
        }

        private Guid ConvertToGuid(object value, bool allowCasting = true)
        {
            Guid guidValue;

            if (value is Guid)
            {
                guidValue = (Guid)value;
            }
            else if (!allowCasting)
            {
                throw new InvalidIdException("Id is not a Guid");
            }
            else
            {
                if (!(value is string))
                    throw new InvalidIdException("Id is not a string or Guid");

                if (!Guid.TryParse(value as string, out guidValue))
                    throw new InvalidIdException("Id is not convertable to a Guid");
            }

            //if (guidValue == Guid.Empty)
            //    throw new InvalidIdException("Id is empty");

            return guidValue;
        }

        private int ConvertToInt32(object value, bool allowCasting = true)
        {
            int intValue;

            if (value is int)
            {
                intValue = (int)value;
            }
            else if (!allowCasting)
            {
                throw new InvalidIdException("Id is not a int");
            }
            else
            {
                if (!(value is string) && !(value is short) && !(value is long))
                    throw new InvalidIdException("Id is not a int, short, long or string");

                if (!int.TryParse(value.ToString(), out intValue))
                    throw new InvalidIdException("Id is not convertable to a int");
            }

            return intValue;
        }

        private string ConvertToString(object value, bool allowCasting = true)
        {
            if (value is string)
                return value as string;
            else if (allowCasting)
                return value.ToString();
            else
                throw new InvalidIdException("Id is not a string");
        }

        internal object GetId()
        {
            //if (Definitions.HasIdProperty)
                return IdValue?.Get();
            //else
            //    return InternalId;
        }

        internal bool HasId()
        {
            var id = GetId();

            if (id == null)
                return false;
            else if (id is Guid)
                return id != null && (Guid)id != Guid.Empty;
            else if (id is int)
                return (int)id != 0;
            else if (id is string)
                return !string.IsNullOrWhiteSpace(id as string);
            else if (id.GetType().IsValueType)
                return id != Activator.CreateInstance(id.GetType());
            else
                return true;
        }

        internal void GenerateId()
        {
            if (Definitions.HasIdProperty && Definitions.IdProperty.PropertyType != typeof(Guid))
                throw new InvalidIdException("Only Id of type Guid is supported");

            SetId(Guid.NewGuid());
        }

        internal string GetValuehash()
        {
            return "";
        }

        public IEnumerable<Container> GetStorage()
        {
            return Definitions.GetStorage(this);
        }

        internal void SetValuesFromStorage(IEnumerable<Container> storage)
        {
            Definitions.SetValuesFromStorage(this, storage);
        }


        internal void SetDefaultValues()
        {
            foreach (var property in Definitions.Properties)
                AddValue(property);
        }


        internal void ReadFromInstance<M>(M m) where M : IModl
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
                SetValue(property.PropertyName, property.GetValue(m));

        }

        internal void ReadFromInstanceId<M>(M m) where M : IModl
        {
            if (Definitions.HasIdProperty)
            {
                var oldValue = GetId();
                var newValue = Definitions.IdProperty.GetValue(m);

                if (!AreEqual(Definitions.IdProperty.PropertyType, newValue, oldValue))
                    SetId(newValue);

                //if ((Definitions.IdProperty.PropertyType == typeof(Guid) && (Guid)newValue != (Guid)oldValue) ||
                //    (Definitions.IdProperty.PropertyType != typeof(Guid) && newValue != oldValue))
                //    SetId(newValue);
            }
        }

        private bool AreEqual(Type type, object a, object b)
        {
            if (a == null && b == null)
                return true;
            else if (a == null)
                return false;
            else if (b == null)
                return false;

            if (a.GetType() != b.GetType())
                return false;
            else if (a.GetType() != type)
                return false;
            else if (b.GetType() != type)
                return false;

            return a.Equals(b);
        }

        internal void WriteToInstance<M>(M m, string propertyName = null) where M : IModl
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsLink && !x.IsId))
                if (propertyName == null || property.PropertyName == propertyName)
                    property.SetValue(m, GetValue<object>(property.PropertyName));
        }

        internal void WriteToInstanceId<M>(M m) where M : IModl
        {
            if (Definitions.HasIdProperty)
                Definitions.IdProperty.SetValue(m, IdValue.Get());
        }

        internal void WriteRelationsToInstance<M>(M m) where M : IModl, new()
        {
            foreach (LinkProperty property in Definitions.Properties.Where(x => x.IsLink))
                property.SetLinkValue(m);
        }

        private object GetDefault(Type type)
        {
            if (!type.IsValueType)
                return null;
            else
                return Activator.CreateInstance(type);
        }

        internal bool Save<M>(M m, bool includeRelations) where M : IModl, new()
        {
            if (IsDeleted)
                throw new NotFoundException(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

            ReadFromInstanceId(m);
            ReadFromInstance(m);

            if (includeRelations)
            {
                foreach (var property in Definitions.Properties.Where(x => x.IsLink))
                {
                    typeof(Backer)
                        .GetMethod("SaveRelation", BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(property.PropertyType)
                        .Invoke(this, new object[] { property });
                }
            }

            if (!HasId() && Definitions.HasAutomaticId)
                GenerateId();
            else if (!HasId())
                throw new InvalidIdException($"Id not set. Class: {ModlType}");


            if (!IsNew && !IsModified())
                return false;

            Materializer.Write(GetStorage(), Settings.Get(ModlType));

            IsNew = false;
            ResetValuesToUnmodified();
            WriteToInstance(m);

            return true;
        }

        internal bool Delete()
        {
            if (IsNew)
                throw new NotFoundException(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", ModlType, GetId()));

            if (IsDeleted)
                throw new NotFoundException(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

            Materializer.Delete(GetStorage(), Settings.Get(ModlType));
            IsDeleted = true;

            return true;
        }
    }
}
