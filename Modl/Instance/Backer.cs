using Modl.Structure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Modl.Structure.Storage;
using System.Reflection;
using Modl.Exceptions;

namespace Modl.Structure.Instance
{
    public class Backer
    {
        public bool IsNew { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        //public bool HasId { get; set; } = false;
        public Guid? InternalId { get; set; }
        public Dictionary<string, IValue> Values { get; set; } = new Dictionary<string, IValue>();
        public Definitions Definitions { get; set; }
        public Type ModlType { get; set; }

        public Backer(Type modlType)
        {
            ModlType = modlType;
            Definitions = Definitions.Get(modlType);
            SetDefaultValues();
        }

        public bool IsModified()
        {
            if (Definitions.HasIdProperty)
            {
                return Values
                    .Where(x => x.Key != Definitions.IdProperty.PropertyName)
                    .Any(x => x.Value.IsModified);
            }
            else
                return Values.Any(x => x.Value.IsModified);
        }

        public T GetValue<T>(string name)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new SimpleValue(default(T));

            return (T)Values[name].Get();
        }

        public void SetValue<T>(string name, T value)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new SimpleValue(value);
            else
                Values[name].Set(value);
        }

        public M GetRelationValue<M>(string name) where M: IModl, new()
        {
            if (!Values.ContainsKey(name))
                Values[name] = new RelationValue(default(M));

            var relationValue = Values[name] as RelationValue;
            if (relationValue.IsLoaded)
                return (M)relationValue.Get();
            else if (relationValue.HasId)
            {
                var m = Handler<M>.Get(relationValue.Id);
                relationValue.Set(m);
                relationValue.IsLoaded = true;
                relationValue.Reset();

                return m;
            }
            else
                return default(M);
        }

        public object GetRelationId(string name)
        {
            if (!Values.ContainsKey(name))
                return null;

            var relationValue = Values[name] as RelationValue;

            return relationValue.Id;
        }

        public bool IsRelationModified<M>(string name) where M : IModl, new()
        {
            if (!Values.ContainsKey(name))
                return false;

            var relationValue = Values[name] as RelationValue;
            if (!relationValue.IsLoaded)
                return false;

            return ((M)relationValue.Get()).IsModified();
        }

        public void SetRelationValue<M>(string name, M value) where M : IModl, new()
        {
            if (!Values.ContainsKey(name))
                Values[name] = new RelationValue(value);
            else
                Values[name].Set(value);

            var relationValue = Values[name] as RelationValue;
            relationValue.IsLoaded = true;
            relationValue.Id = value.Id().Get();
        }

        public void SetRelationId(string name, object id)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new RelationValue(null);

            var relationValue = Values[name] as RelationValue;
            relationValue.Id = id;
            relationValue.HasId = true;
            relationValue.IsLoaded = false;
        }

        internal void ResetValuesToUnmodified()
        {
            foreach (var value in Values.Values)
                value.Reset();
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

                SetValue(id.PropertyName, value);
            }
            else
            {
                InternalId = ConvertToGuid(value);
            }

            //HasId = true;
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
            if (Definitions.HasIdProperty)
                return GetValue<object>(Definitions.IdProperty.PropertyName);
            else
                return InternalId;
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
            foreach (var property in Definitions.Properties.Where(x => !x.IsRelation))
                SetValue(property.PropertyName, GetDefault(property.PropertyType));
        }


        internal void ReadFromInstance<M>(M m) where M: IModl
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsRelation && !x.IsId))
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
            foreach (var property in Definitions.Properties.Where(x => !x.IsRelation))
                if (propertyName == null || property.PropertyName == propertyName)
                    property.SetValue(m, GetValue<object>(property.PropertyName));
        }

        internal void WriteToInstanceId<M>(M m) where M : IModl
        {
            if (Definitions.HasIdProperty)
                WriteToInstance(m, Definitions.IdProperty.PropertyName);
        }

        private object GetDefault(Type type)
        {
            if (!type.IsValueType)
                return null;
            else
                return Activator.CreateInstance(type);
        }

        internal void SaveRelation<M>(Property property) where M : IModl, new()
        {
            if (IsRelationModified<M>(property.PropertyName))
                GetValue<M>(property.PropertyName).Save();
        }

        internal bool Save<M>(M m, bool includeRelations) where M : IModl, new()
        {
            if (IsDeleted)
                throw new NotFoundException(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

            ReadFromInstanceId(m);
            ReadFromInstance(m);

            if (includeRelations)
            {
                foreach (var property in Definitions.Properties.Where(x => x.IsRelation))
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
