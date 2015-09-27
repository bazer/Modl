﻿/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using Modl.Structure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Modl.Structure.Storage;

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

        public bool IsModified<M>(M m) where M: IModl
        {
            ReadFromInstance(m);
            return Values.Values.Any(x => x.IsModified);
        }

        //public T GetValue<T>(string name)
        //{
        //    return (T)GetField<T>(name).Get();
        //}

        //public void SetValue<T>(string name, T value)
        //{
        //    SetField<T>(name, value);
        //}

        public T GetValue<T>(string name)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new SimpleValue(default(T));

            return (T)Values[name].Get();
        }

        public void SetValue<T>(string name, T value)
        {
            //if (Definitions.Properties.Single(x => x.PropertyName == name).IsRelation)
            //    SetRelationValue(name, value);

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
                var m = Handler<M>.Get<M>(relationValue.Id);
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

        internal void ResetValuesToUnmodified()
        {
            foreach (var value in Values.Values)
                value.Reset();
        }


        internal void SetId(object value)
        {
            if (Definitions.HasPrimaryKey)
            {
                if (Definitions.PrimaryKey.PropertyType == typeof(Guid))
                    value = Guid.Parse(value.ToString());
                else if (Definitions.PrimaryKey.PropertyType == typeof(int))
                    value = int.Parse(value.ToString());
                else if (Definitions.PrimaryKey.PropertyType == typeof(string))
                    value = value.ToString();
                else
                    throw new NotSupportedException("Unsupported Id type");

                SetValue(Definitions.PrimaryKey.PropertyName, value);

                //WriteToInstance(Metadata.PrimaryKey.Name);
            }
            else
            {
                InternalId = value;
            }
        }

        internal object GetId()
        {
            if (Definitions.HasPrimaryKey)
                return GetValue<object>(Definitions.PrimaryKey.PropertyName);
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

        private object GetDefault(Type type)
        {
            if (!type.IsValueType)
                return null;
            else
                return Activator.CreateInstance(type);
        }
    }
}
