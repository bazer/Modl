using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Modl.Instance;

namespace Modl.Metadata
{
    public class Layer
    {
        public string Name { get; set; }
        public string ModlName { get; private set; }
        internal Type Type { get; set; }
        //internal Layer Parent { get; set; }
        //internal bool HasParent => Parent != null;
        internal bool HasIdProperty => Properties.Any(x => x.IsId);
        internal bool HasAutomaticId => !HasIdProperty || (HasIdProperty && IdProperty.IsAutomaticId);

        internal List<Property> Properties { get; private set; }
        internal List<Property> AllProperties { get; private set; }

        public Layer(Type type)
        {
            //Disable inheritance for now
            //if (type.BaseType != null && type.BaseType != typeof(object))
            //    Parent = new Layer(type.BaseType);

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
                if (info.CanWrite && !typeof(IModlData).IsAssignableFrom(info.PropertyType) && !info.GetMethod.IsAbstract)
                {
                    Properties.Add(PropertyFactory.Create(info, Type));
                }
            }

            //if (HasParent)
            //    AllProperties = Properties.Concat(Parent.AllProperties.Where(x => !Properties.Any(y => y.PropertyName == x.PropertyName))).ToList();
            //else
            AllProperties = Properties;
        }

        internal Property IdProperty
        {
            get
            {
                return Properties.SingleOrDefault(x => x.IsId);
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
            //if (HasParent)
            //    Parent.SetValuesFromStorage(instance, storage);

            foreach (var value in storage.Single(x => x.About.Type == ModlName).Values)
            {
                var property = GetPropertyFromModlName(value.Key);

                if (property.IsId)
                    continue;

                var newValue = value.Value;

                if (value.Value != null && !property.PropertyType.IsInstanceOfType(value.Value))
                    newValue = Materializer.DeserializeObject(value.Value, property.PropertyType, Settings.Get(Type));

                if (property.IsLink)
                    instance.GetRelation(property.PropertyName).Set(newValue as IEnumerable<RelationIdValue>);
                else
                    instance.SetValue(property.PropertyName, newValue);
            }
        }

        public IEnumerable<Container> GetStorage(Backer instance)
        {
            yield return new Container(GetAbout(instance), GetValues(instance))
            {
                Identity = GetIdentity(instance.GetId())
            };

            //if (HasParent)
            //    foreach (var x in Parent.GetStorage(instance))
            //        yield return x;
        }

        internal About GetAbout(Backer instance)
        {
            return new About
            {
                Id = instance.GetId(), //.ToString(), //instance.GetValue<object>(PrimaryKey.PropertyName).ToString(),
                Type = ModlName,
                Time = DateTime.UtcNow
            };
        }

        internal StorageIdentity GetIdentity(object id)
        {
            return new StorageIdentity
            {
                Id = id,
                IdType = HasIdProperty ? IdProperty.PropertyType : typeof(Guid),
                Name = ModlName,
                Type = Type
            };
        }

        internal IEnumerable<StorageIdentity> GetIdentities(object id)
        {
            yield return GetIdentity(id);

            //if (HasParent)
            //    foreach (var x in Parent.GetIdentities(id))
            //        yield return x;
        }


        private Dictionary<string, object> GetValues(Backer backer)
        {
            return Properties.Select(x =>
            {

                object value;

                if (x.IsLink)
                    value = backer.GetRelation(x.PropertyName).Get();
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
