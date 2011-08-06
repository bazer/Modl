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
        void SetValue<T>(string name, T value);
    }

    internal class Store<M> : IStore
        where M : Modl<M>, new()
    {
        internal Type IdType { get; set; }
        protected object id; // = default(IdType);
        internal object Id { get { return id; } set { id = value; } }

        public DynamicFields<M> DynamicFields;
        protected Dictionary<string, Field> Fields = new Dictionary<string, Field>();
        string NameOfLastInsertedMember;

        public Store(Type IdType)
        {
            this.IdType = IdType;
            DynamicFields = new DynamicFields<M>(this);
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
            NameOfLastInsertedMember = name;
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

        public string LastInsertedMemberName
        {
            get
            {
                return NameOfLastInsertedMember;
            }
        }

        internal bool Load(DbDataReader reader, bool throwExceptionOnNotFound = true, bool singleRow = true)
        {
            if (reader.Read())
            {
                id = Helper.GetSafeValue(reader, Statics<M>.IdName, IdType);

                var keys = Fields.Keys.ToList();

                for (int i = 0; i < Fields.Count; i++)
                {
                    string key = keys[i];

                    if (Fields[key].Type.GetInterface("IModl") != null)
                        SetField(key, Helper.GetSafeValue(reader, key, typeof(int?)));
                    else
                        SetField(key, Helper.GetSafeValue(reader, key, Statics<M>.GetFieldType(key)));
                }

                if (singleRow)
                    reader.Close();

                return true;
            }
            else
            {
                reader.Close();

                if (singleRow && throwExceptionOnNotFound)
                    throw new RecordNotFoundException();
                else
                    return false;
            }
        }

        internal void BaseAddSaveFields(Change<M> statement)
        {
            foreach (var field in Fields)
            {
                if (field.Value.Type.GetInterface("IModl") != null && !(field.Value.Value is int))
                {
                    var value = field.Value as IModl;

                    if (value == null)
                        statement.With(field.Key, null);
                    else if (value.Id == 0)
                        throw new Exception("Can't save foreign key of unsaved object: " + value);
                    else
                        statement.With(field.Key, value.Id);
                }
                else
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
