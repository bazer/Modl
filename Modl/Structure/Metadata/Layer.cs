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
    public class Layer<M>
        where M : IModl, new()
    {
        public string Name { get; set; }
        public string ModlName { get; private set; }
        internal Type Type { get; set; }
        internal Layer<M> Parent { get; set; }
        internal bool HasParent => Parent != null;
        internal bool HasPrimaryKey => Properties.Any(x => x.IsPrimaryKey);

        internal List<Property<M>> Properties { get; private set; }
        internal List<Property<M>> AllProperties { get; private set; }

        public Layer(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
                Parent = new Layer<M>(type.BaseType);

            Properties = new List<Property<M>>();

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
                if (info.CanWrite)
                {
                    var property = new Property<M>(info, this);
                    Properties.Add(property);
                }
            }

            if (HasParent)
                AllProperties = Properties.Concat(Parent.AllProperties).ToList();
            else
                AllProperties = Properties;
        }

        internal Property<M> PrimaryKey
        {
            get
            {
                return Properties.Single(x => x.IsPrimaryKey);
            }
        }

        internal IEnumerable<Property<M>> ForeignKeys
        {
            get
            {
                return Properties.Where(x => x.IsForeignKey);
            }
        }

        internal void SetValuesFromStorage(Instance<M> instance, IEnumerable<Storage.Storage> storage)
        {
            if (HasParent)
                Parent.SetValuesFromStorage(instance, storage);

            foreach (var value in storage.Single(x => x.About.Type == ModlName).Values)
            {
                var property = GetPropertyFromModlName(value.Key);
                var newValue = value.Value;

                if (property.Name == PrimaryKey.Name)
                    instance.SetId(newValue);
                else
                {
                    if (value.Value != null && !property.Type.IsInstanceOfType(value.Value))
                        newValue = Materializer.DeserializeObject(value.Value, property.Type, Internal<M>.Settings);

                    instance.SetValue(property.Name, newValue);
                }
            }
        }

        public IEnumerable<Storage.Storage> GetStorage(Instance<M> instance)
        {
            yield return new Storage.Storage(GetAbout(instance), GetValues(instance))
            {
                Identity = GetIdentity(instance.GetValue<object>(PrimaryKey.Name))
            };

            if (HasParent)
                foreach (var x in Parent.GetStorage(instance))
                    yield return x;
        }

        internal About GetAbout(Instance<M> instance)
        {
            return new About
            {
                Id = instance.GetValue<object>(PrimaryKey.Name).ToString(),
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


        private Dictionary<string, object> GetValues(Instance<M> instance)
        {
            return Properties.Select(x =>
            {
                var value = instance.GetValue<object>(x.Name);

                if (value != null)
                {
                    if (typeof(IModl).IsAssignableFrom(x.Type))
                        value = null;
                }

                return new KeyValuePair<string, object>(x.ModlName, value);
            })
            .ToDictionary(x => x.Key, x => x.Value);
        }

        private string GetPropertyModlName(string name)
        {
            return Properties.Single(x => x.Name == name).ModlName;
        }

        private Property<M> GetPropertyFromModlName(string modlName)
        {
            return Properties.Single(x => x.ModlName == modlName);
        }
    }
}
