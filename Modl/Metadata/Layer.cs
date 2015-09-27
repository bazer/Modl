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
using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Modl.Structure.Instance;

namespace Modl.Structure.Metadata
{
    public class Layer
    {
        public string Name { get; set; }
        public string ModlName { get; private set; }
        internal Type Type { get; set; }
        internal Layer Parent { get; set; }
        internal bool HasParent => Parent != null;
        internal bool HasPrimaryKey => Properties.Any(x => x.IsPrimaryKey);
        internal bool HasAutomaticKey => !HasPrimaryKey || (HasPrimaryKey && PrimaryKey.AutomaticKey);

        internal List<Property> Properties { get; private set; }
        internal List<Property> AllProperties { get; private set; }

        public Layer(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
                Parent = new Layer(type.BaseType);

            Properties = new List<Property>();

            Name = type.Name;
            ModlName = type.Name;
            Type = type;

            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    ModlName = ((NameAttribute)attribute).Name;
                //else if (attribute is CacheAttribute)
                //{
                //    Settings.CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                //    Settings.CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                //}
            }

            foreach (PropertyInfo info in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (info.CanWrite && !typeof(IModlData).IsAssignableFrom(info.PropertyType))
                {
                    var property = new Property(info, this);
                    Properties.Add(property);
                }
            }

            if (HasParent)
                AllProperties = Properties.Concat(Parent.AllProperties).ToList();
            else
                AllProperties = Properties;
        }

        internal Property PrimaryKey
        {
            get
            {
                return Properties.Single(x => x.IsPrimaryKey);
            }
        }

        

        //internal IEnumerable<Property> ForeignKeys
        //{
        //    get
        //    {
        //        return Properties.Where(x => x.IsForeignKey);
        //    }
        //}

        internal void SetValuesFromStorage(Backer instance, IEnumerable<Container> storage)
        {
            if (HasParent)
                Parent.SetValuesFromStorage(instance, storage);

            foreach (var value in storage.Single(x => x.About.Type == ModlName).Values)
            {
                var property = GetPropertyFromModlName(value.Key);
                var newValue = value.Value;

                if (property.PropertyName == PrimaryKey.PropertyName)
                    instance.SetId(newValue);
                else
                {
                    if (value.Value != null && !property.PropertyType.IsInstanceOfType(value.Value))
                        newValue = Materializer.DeserializeObject(value.Value, property.PropertyType, Settings.Get(Type));

                    instance.SetValue(property.PropertyName, newValue);
                }
            }
        }

        public IEnumerable<Container> GetStorage(Backer instance)
        {
            yield return new Container(GetAbout(instance), GetValues(instance))
            {
                Identity = GetIdentity(instance.GetId())
            };

            if (HasParent)
                foreach (var x in Parent.GetStorage(instance))
                    yield return x;
        }

        internal About GetAbout(Backer instance)
        {
            return new About
            {
                Id = instance.GetId().ToString(), //instance.GetValue<object>(PrimaryKey.PropertyName).ToString(),
                Type = ModlName,
                Time = DateTime.UtcNow
            };
        }

        internal Identity GetIdentity(object id)
        {
            return new Identity
            {
                Id = id.ToString(),
                Name = ModlName,
                Type = Type
            };
        }

        internal IEnumerable<Identity> GetIdentities(object id)
        {
            yield return GetIdentity(id);

            if (HasParent)
                foreach (var x in Parent.GetIdentities(id))
                    yield return x;
        }


        private Dictionary<string, object> GetValues(Backer backer)
        {
            return Properties.Select(x =>
            {

                object value;

                if (x.IsRelation)
                    value = backer.GetRelationId(x.PropertyName);
                else
                    value = backer.GetValue<object>(x.PropertyName);

                //if (value != null)
                //{
                //    if (typeof(IModl).IsAssignableFrom(x.PropertyType))
                //        value = null;
                //}

                return new KeyValuePair<string, object>(x.StorageName, value);
            })
            .ToDictionary(x => x.Key, x => x.Value);
        }

        private string GetPropertyModlName(string name)
        {
            return Properties.Single(x => x.PropertyName == name).StorageName;
        }

        private Property GetPropertyFromModlName(string modlName)
        {
            return Properties.Single(x => x.StorageName == modlName);
        }
    }
}
