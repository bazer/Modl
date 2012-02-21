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
using System.Text;
using System.Data.Common;
using Modl.Exceptions;
using System.Reflection;
using Modl.Query;
using Modl;

namespace Modl.Fields
{
    interface IContent
    {
        T GetValue<T>(string name);
        void SetValue<T>(string name, T value);
    }

    internal class Content : IContent
    {
        //protected IModl instance;

        //protected object id;
        //internal object Id { get { return id; } set { id = value; } }

        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool AutomaticId { get; set; }
        public Database Database { get; set; }
        public Dictionary<string, Field> Fields = new Dictionary<string, Field>();
        
        
        private static Dictionary<int, Content> Contents = new Dictionary<int, Content>();
        

        public Content()
        {
            //this.instance = instance;
            //this.Database = database;
            IsNew = true;
            AutomaticId = true;
        }

        internal static Content GetContents(IModl instance)
        {
            Content content;
            if (!Contents.TryGetValue(instance.GetHashCode(), out content))
                return null;

            return content;
        }

        internal static Content AddInstance(IModl instance)
        {
            var content = new Content();
            Contents.Add(instance.GetHashCode(), content);

            return content;
        }

        public bool IsDirty
        {
            get
            {
                return Fields.Values.Any(x => x.IsDirty);
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

        protected Field GetField<T>(string name)
        {
            if (!Fields.ContainsKey(name))
                Fields[name] = new Field(default(T), typeof(T));

            return (Field)Fields[name];
        }

        protected void SetField<T>(string name, T value)
        {
            if (!Fields.ContainsKey(name))
                Fields[name] = new Field(value, typeof(T));
            else
                Fields[name].Value = value;
        }

        

        //internal string LastInsertedMemberName { get; set; }

        internal void ResetFields()
        {
            foreach (var field in Fields.Values)
                field.Reset();
        }
    }
}
