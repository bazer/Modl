/*
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
    public class Instance<M>
        where M : IModl, new()
    {
        public M Instance { get; set; }

        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool AutomaticId { get; set; }
        public string InternalId { get; set; }
        public Dictionary<string, InstanceValue> Values { get; set; }
        public Metadata<M> Metadata { get { return Internal<M>.Metadata; } }


        public Instance(M instance)
        {
            this.Instance = instance;
            Values = new Dictionary<string, InstanceValue>();

            IsNew = true;
            AutomaticId = true;

            SetDefaultValues();
        }

        public bool IsModified
        {
            get
            {
                ReadFromInstance();
                return Values.Values.Any(x => x.IsModified);
            }
        }

        public T GetValue<T>(string name)
        {
            return (T)GetField<T>(name).Value;
        }

        public void SetValue<T>(string name, T value)
        {
            SetField<T>(name, value);
        }

        public InstanceValue GetField<T>(string name)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new InstanceValue(default(T), typeof(T));

            return (InstanceValue)Values[name];
        }

        protected void SetField<T>(string name, T value)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new InstanceValue(value, typeof(T));
            else
                Values[name].Value = value;
        }

        internal void ResetFields()
        {
            foreach (var field in Values.Values)
                field.Reset();
        }


        internal void SetId(object value)
        {
            if (Metadata.HasPrimaryKey)
            {
                if (Metadata.PrimaryKey.Type == typeof(Guid))
                    value = Guid.Parse(value.ToString());
                else if (Metadata.PrimaryKey.Type == typeof(int))
                    value = int.Parse(value.ToString());
                else if (Metadata.PrimaryKey.Type == typeof(string))
                    value = value.ToString();
                else
                    throw new NotSupportedException("Unsupported Id type");

                SetValue(Metadata.PrimaryKey.Name, value);
                AutomaticId = false;

                WriteToInstance(Metadata.PrimaryKey.Name);
            }
            else
            {
                InternalId = Guid.NewGuid().ToString();
            }
        }

        internal object GetId()
        {
            if (Metadata.HasPrimaryKey)
                return GetValue<object>(Metadata.PrimaryKey.Name);
            else
                return InternalId;
        }

        internal string GetValuehash()
        {
            return "";
        }

        public IEnumerable<Storage.Storage> GetStorage()
        {
            return Metadata.GetStorage(this);
        }

        internal void SetValuesFromStorage(IEnumerable<Storage.Storage> storage)
        {
            Metadata.SetValuesFromStorage(this, storage);
        }


        internal void SetDefaultValues()
        {
            foreach (var property in Metadata.Properties)
                SetValue(property.Name, GetDefault(property.Type));
        }


        internal void ReadFromInstance()
        {
            foreach (var property in Metadata.Properties)
                SetValue(property.Name, property.GetValue(Instance));
                
        }

        internal void WriteToInstance(string propertyName = null)
        {
            foreach (var property in Metadata.Properties)
                if (propertyName == null || property.Name == propertyName)
                    property.SetValue(Instance, GetValue<object>(property.Name));
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
