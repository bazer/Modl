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
using System.Reflection;
using System.Text;

namespace Modl.Structure.Metadata
{
    public class ModlLayer<M>
        where M : IModl, new()
    {
        public string Name { get; set; }
        public string ModlName { get; private set; }
        internal Type Type { get; set; }
        internal ModlLayer<M> Parent { get; set; }
        internal bool HasParent { get { return Parent != null; } }

        internal List<ModlProperty<M>> Properties { get; private set; }
        internal List<ModlProperty<M>> AllProperties { get; private set; }

        //internal Dictionary<string, Type> Fields = new Dictionary<string, Type>();
        //internal Dictionary<string, Type> Keys = new Dictionary<string, Type>();
        //internal Dictionary<string, Type> ForeignKeys = new Dictionary<string, Type>();

        public ModlLayer(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
                Parent = new ModlLayer<M>(type.BaseType);

            //Layers.Insert(0, new ModlLayer<M>(type));

            Properties = new List<ModlProperty<M>>();

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
                    var property = new ModlProperty<M>(info, this);
                    Properties.Add(property);
                }
            }
            
            if (HasParent)
                AllProperties = Properties.Concat(Parent.AllProperties).ToList();
            else
                AllProperties = Properties;
        }

        //internal bool HasKey
        //{
        //    get
        //    {
        //        return Keys.Count != 0;
        //    }
        //}

        internal ModlProperty<M> PrimaryKey
        {
            get
            {
                return Properties.Single(x => x.IsPrimaryKey);

                //if (Keys.Count != 0)
                //    return Keys.First();
                //else if (ForeignKeys.Count != 0)
                //    return ForeignKeys.First();

                //throw new Exception("Table " + Name + " has no primary key");
            }
        }

        internal IEnumerable<ModlProperty<M>> ForeignKeys
        {
            get
            {
                return Properties.Where(x => x.IsForeignKey);
                    

                //if (Keys.Count != 0)
                //    return Keys.First();
                //else if (ForeignKeys.Count != 0)
                //    return ForeignKeys.First();

                //throw new Exception("Table " + Name + " has no primary key");
            }
        }

        //public string PrimaryKeyName
        //{
        //    get
        //    {
        //        return PrimaryKey.Key;
        //    }
        //}

        //internal Type PrimaryKeyType
        //{
        //    get
        //    {
        //        return PrimaryKey.Value;
        //    }
        //}
    }
}
