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
    internal class Store<C> where C : Modl<C>, new()
    {
        public string IdName;
        protected int id = 0;
        internal int Id { get { return id; } set { id = value; } }

        public DynamicFields<C> DynamicFields;
        public Dictionary<string, Field> Fields = new Dictionary<string, Field>();
        

        string NameOfLastInsertedMember;

        public Store(string idName)
        {
            IdName = idName;
            DynamicFields = new DynamicFields<C>(this);
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
                id = Helper.GetSafeValue(reader, IdName, 0);

                var keys = Fields.Keys.ToList();

                for (int i = 0; i < Fields.Count; i++)
                {
                    string key = keys[i];

                    if (Fields[key].Type.GetInterface("IModl") != null)
                        SetField(key, Helper.GetSafeValue(reader, key, Fields[key].Value, typeof(int?)));
                    else
                        SetField(key, Helper.GetSafeValue(reader, key, Fields[key].Value, Fields[key].Type));
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

        internal virtual void SetDefaults(Modl<C> instance)
        {
            foreach (PropertyInfo property in typeof(C).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(instance, Helper.GetDefault(property.PropertyType), null);
                    Fields[LastInsertedMemberName].Type = property.PropertyType;
                    Statics<C>.SetFieldName(property.Name, LastInsertedMemberName);
                }
            }
        }

        internal void BaseAddSaveFields(Change<C> statement)
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
