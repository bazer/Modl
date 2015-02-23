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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
//using Modl.Query;

namespace Modl.Structure
{
    //interface IModlInstance
    //{
    //    T GetValue<T>(string name);
    //    void SetValue<T>(string name, T value);
    //}

    public class ModlInstance<M> //: IModlInstance
        where M : IModl, new()
    {
        //protected IModl instance;

        //protected object id;
        //internal object Id { get { return id; } set { id = value; } }

        public M Instance { get; set; }

        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool AutomaticId { get; set; }
        //public Database Database { get; set; }
        public Dictionary<string, ModlProperty> Properties { get; set; }

        public ModlMetadata<M> Metadata { get { return ModlInternal<M>.Metadata; } }


        public ModlInstance(M instance)
        {
            this.Instance = instance;
            Properties = new Dictionary<string, ModlProperty>();

            IsNew = true;
            AutomaticId = true;

            FillFields();
        }

        public bool IsModified
        {
            get
            {
                ReadFromEmptyProperties();
                return Properties.Values.Any(x => x.IsModified);
            }
        }

        public T GetValue<T>(string name)
        {
            return (T)GetField<T>(name).Value;
        }

        public void SetValue<T>(string name, T value)
        {
            SetField<T>(name, value);
            //LastInsertedMemberName = name;
        }

        public ModlProperty GetField<T>(string name)
        {
            if (!Properties.ContainsKey(name))
                Properties[name] = new ModlProperty(default(T), typeof(T));

            return (ModlProperty)Properties[name];
        }

        protected void SetField<T>(string name, T value)
        {
            if (!Properties.ContainsKey(name))
                Properties[name] = new ModlProperty(value, typeof(T));
            else
                Properties[name].Value = value;
        }

        

        //internal string LastInsertedMemberName { get; set; }

        internal void ResetFields()
        {
            foreach (var field in Properties.Values)
                field.Reset();
        }


        internal void SetId(object value)
        {
            if (Metadata.IdType == typeof(Guid))
                value = Guid.Parse(value.ToString());
            else if (Metadata.IdType == typeof(int))
                value = int.Parse(value.ToString());
            else if (Metadata.IdType == typeof(string))
                value = value.ToString();

            SetValue<object>(Metadata.IdName, value);
            AutomaticId = false;

            WriteToEmptyProperties(Metadata.IdName);
        }

        internal object GetId()
        {
            return GetValue<object>(Metadata.IdName);
        }

        internal ModlIdentity GetIdentity()
        {
            return new ModlIdentity
            {
                Id = GetId().ToString(),
                Name = Metadata.ModlName,
                Timestamp = DateTime.UtcNow,
                Version = 0,
                Valuehash = GetValuehash()
            };
        }

        internal string GetValuehash()
        {
            return "";
        }

        internal Dictionary<string, object> GetValues()
        {
            return Properties
                .Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public ModlStorage GetStorage()
        {
            return new ModlStorage(GetIdentity(), GetValues());
        }

        internal void SetValuesFromStorage(ModlStorage storage)
        {
            foreach (var value in storage.Values)
            {
                if (value.Key == Metadata.IdName)
                    SetId(value.Value);
                else
                    SetValue(value.Key, value.Value);
            }
        }


        internal void FillFields()
        {
            foreach (var field in Metadata.Types)
                SetValue(field.Key, GetDefault(field.Value));
        }


        internal void ReadFromEmptyProperties()
        {
            foreach (var property in Metadata.EmptyProperties)
                SetValue(Metadata.Properties[property.Item1.Name], property.Item2(Instance));
        }

        internal void WriteToEmptyProperties(string propertyName = null)
        {
            foreach (var property in Metadata.EmptyProperties)
                if (propertyName == null || property.Item1.Name == propertyName)
                    property.Item3(Instance, GetValue<object>(Metadata.Properties[property.Item1.Name]));
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
