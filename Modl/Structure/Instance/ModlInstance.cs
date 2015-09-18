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
        public string InternalId { get; set; }
        //public Database Database { get; set; }
        public Dictionary<string, ModlValue> Values { get; set; }

        public ModlMetadata<M> Metadata { get { return ModlInternal<M>.Metadata; } }


        public ModlInstance(M instance)
        {
            this.Instance = instance;
            Values = new Dictionary<string, ModlValue>();

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
            //LastInsertedMemberName = name;
        }

        public ModlValue GetField<T>(string name)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new ModlValue(default(T), typeof(T));

            return (ModlValue)Values[name];
        }

        protected void SetField<T>(string name, T value)
        {
            if (!Values.ContainsKey(name))
                Values[name] = new ModlValue(value, typeof(T));
            else
                Values[name].Value = value;
        }

        

        //internal string LastInsertedMemberName { get; set; }

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

        //internal ModlIdentity GetIdentity()
        //{
        //    return new ModlIdentity
        //    {
        //        Id = GetId().ToString(),
        //        Name = Metadata.ModlName,
        //        Time = DateTime.UtcNow,
        //        Version = 0
        //    };
        //}

        internal string GetValuehash()
        {
            return "";
        }

        //internal Dictionary<string, object> GetValues()
        //{
        //    foreach (var property in Metadata.Properties)
        //    {

        //    }

        //    Metadata.Properties.Select(x =>
        //    {
        //        var modlValue = Values[x.Name];
        //        object outputValue;

        //        if (x.IsForeignKey && modlValue)
        //            outputValue = modlValue.Value;
        //    }

        //    return Values
        //        .Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Value))
        //        .ToDictionary(x => x.Key, x => x.Value);
        //}

        public IEnumerable<ModlStorage> GetStorage()
        {
            return Metadata.GetStorage(this);
        }

        internal void SetValuesFromStorage(IEnumerable<ModlStorage> storage)
        {
            Metadata.SetValuesFromStorage(this, storage);

            //foreach (var value in storage.SelectMany(x => x).Values)
            //{
            //    if (value.Key == Metadata.PrimaryKey.Name)
            //        SetId(value.Value);
            //    else
            //        SetValue(value.Key, value.Value);
            //}
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
