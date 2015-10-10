using Modl.Structure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Modl.Structure.Storage;
using System.Reflection;

namespace Modl.Structure.Instance
{
    public class Backer
    {
        public bool IsNew { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public object InternalId { get; set; }
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
            if (Definitions.HasId)
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
            relationValue.Id = value.GetId();
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


        internal void SetId(object value)
        { 
            if (Definitions.HasId)
            {
                var id = Definitions.IdProperty;

                if (id.PropertyType == typeof(Guid))
                    value = Guid.Parse(value.ToString());
                else if (id.PropertyType == typeof(int))
                    value = int.Parse(value.ToString());
                else if (id.PropertyType == typeof(string))
                    value = value.ToString();
                else
                    throw new NotSupportedException("Unsupported Id type");

                SetValue(id.PropertyName, value);
            }
            else
            {
                InternalId = value;
            }
        }

        internal object GetId()
        {
            if (Definitions.HasId)
                return GetValue<object>(Definitions.IdProperty.PropertyName);
            else
                return InternalId;
        }

        internal bool HasId()
        {
            return GetId() != null;
        }

        internal void GenerateId()
        {
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
            foreach (var property in Definitions.Properties.Where(x => !x.IsRelation))
                SetValue(property.PropertyName, property.GetValue(m));
                
        }

        internal void WriteToInstance<M>(M m, string propertyName = null) where M : IModl
        {
            foreach (var property in Definitions.Properties.Where(x => !x.IsRelation))
                if (propertyName == null || property.PropertyName == propertyName)
                    property.SetValue(m, GetValue<object>(property.PropertyName));
        }

        internal void WriteToInstanceId<M>(M m) where M : IModl
        {
            if (Definitions.HasId)
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
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

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


            if (!IsModified())
                return false;

            if (!HasId() && Definitions.HasAutomaticId)
                GenerateId();
            else if (!HasId())
                throw new Exception($"Id not set. Class: {ModlType}");

            Materializer.Write(GetStorage(), Settings.Get(ModlType));

            IsNew = false;
            ResetValuesToUnmodified();
            WriteToInstance(m);

            return true;
        }

        internal bool Delete()
        {
            if (IsNew)
                throw new Exception(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", ModlType, GetId()));

            if (IsDeleted)
                throw new Exception(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", ModlType, GetId()));

            Materializer.Delete(GetStorage(), Settings.Get(ModlType));
            IsDeleted = true;

            return true;
        }
    }
}
