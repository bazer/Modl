/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

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

namespace Modl.Fields
{
    interface IStore
    {
        T GetValue<T>(string name);
        void SetValue<T>(string name, T value, bool emptyProperty = false);
    }

    internal class Store<M, IdType> : IStore
        where M : Modl<M, IdType>, new()
    {
        protected Modl<M, IdType> instance;

        protected IdType id;
        internal IdType Id { get { return id; } set { id = value; } }

        public DynamicFields<M, IdType> DynamicFields;
        protected Dictionary<string, Field> Fields = new Dictionary<string, Field>();

        public Store(Modl<M, IdType> instance)
        {
            this.instance = instance;

            DynamicFields = new DynamicFields<M, IdType>(this);
        }

        public bool IsDirty
        {
            get
            {
                ReadFromEmptyProperties();
                return Fields.Values.Any(x => x.IsDirty);
            }
        }

        protected void ReadFromEmptyProperties()
        {
            Statics<M, IdType>.ReadFromEmptyProperties(instance);
        }

        protected void WriteToEmptyProperties()
        {
            Statics<M, IdType>.WriteToEmptyProperties(instance);
        }

        public T GetValue<T>(string name)
        {
            return (T)GetField<T>(name).Value;
        }

        public void SetValue<T>(string name, T value, bool emptyProperty = false)
        {
            SetField<T>(name, value, emptyProperty);
            LastInsertedMemberName = name;
        }

        protected Field GetField<T>(string name)
        {
            if (!Fields.ContainsKey(name))
                Fields[name] = new Field(default(T), typeof(T));

            return (Field)Fields[name];
        }

        protected void SetField<T>(string name, T value, bool emptyProperty = false)
        {
            if (!Fields.ContainsKey(name))
                Fields[name] = new Field(value, typeof(T), emptyProperty);
            else
                Fields[name].Value = value;
        }

        internal string LastInsertedMemberName { get; set; }

        internal void Load(DbDataReader reader)
        {
            id = Helper.GetSafeValue<IdType>(reader, Statics<M, IdType>.IdName);

            var keys = Fields.Keys.ToList();

            for (int i = 0; i < Fields.Count; i++)
            {
                string key = keys[i];

                //if (Fields[key].Type.GetInterface("IModl") != null)
                //    SetField(key, Helper.GetSafeValue(reader, key, typeof(int?)));
                //else
                SetField(key, Helper.GetSafeValue(reader, key, Statics<M, IdType>.GetFieldType(key)));
            }

            WriteToEmptyProperties();
        }

        internal void BaseAddSaveFields(Change<M, IdType> statement)
        {
            ReadFromEmptyProperties();

            foreach (var field in Fields)
            {
                //if (field.Value.Type.GetInterface("IModl") != null && !(field.Value.Value is int))
                //{
                //    var value = field.Value as IModl;

                //    if (value == null)
                //        statement.With(field.Key, null);
                //    else if (value.Id == 0)
                //        throw new Exception("Can't save foreign key of unsaved object: " + value);
                //    else
                //        statement.With(field.Key, value.Id);
                //}
                //else 
                if (field.Value.IsDirty)
                    statement.With(field.Key, field.Value.Value);
            }
        }

        internal void ResetFields()
        {
            foreach (var field in Fields.Values)
                field.Reset();
        }
    }
}
