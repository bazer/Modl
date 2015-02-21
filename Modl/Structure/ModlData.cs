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
using System.Collections.Generic;
using System.Linq;
//using Modl.Query;

namespace Modl.Structure
{
    interface IContent
    {
        T GetValue<T>(string name);
        void SetValue<T>(string name, T value);
    }

    public class ModlData : IContent
    {
        //protected IModl instance;

        //protected object id;
        //internal object Id { get { return id; } set { id = value; } }

        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool AutomaticId { get; set; }
        //public Database Database { get; set; }
        public Dictionary<string, ModlProperty> Fields = new Dictionary<string, ModlProperty>();
        
        
        private static Dictionary<int, ModlData> Contents = new Dictionary<int, ModlData>();
        

        public ModlData()
        {
            //this.instance = instance;
            //this.Database = database;
            IsNew = true;
            AutomaticId = true;
        }

        internal static ModlData GetContents(IModl instance)
        {
            ModlData content;
            if (!Contents.TryGetValue(instance.GetHashCode(), out content))
                return null;

            return content;
        }

        public static ModlData AddInstance(IModl instance)
        {
            var content = new ModlData();
            Contents.Add(instance.GetHashCode(), content);

            return content;
        }

        public static bool HasInstance(IModl instance)
        {
            return Contents.ContainsKey(instance.GetHashCode());
        }

        public bool IsModified
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

        public ModlProperty GetField<T>(string name)
        {
            if (!Fields.ContainsKey(name))
                Fields[name] = new ModlProperty(default(T), typeof(T));

            return (ModlProperty)Fields[name];
        }

        protected void SetField<T>(string name, T value)
        {
            if (!Fields.ContainsKey(name))
                Fields[name] = new ModlProperty(value, typeof(T));
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
