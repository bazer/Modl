using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Modl.Exceptions;
using System.Reflection;

namespace Modl.Fields
{
    internal class Store<C> where C : Modl<C>, new()
    {
        public string IdName;
        protected int id = 0;
        internal int Id { get { return id; } set { id = value; } }

        public DynamicFields<C> Fields;
        public Dictionary<string, object> Dictionary = new Dictionary<string, object>();
        public Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public Store(string idName)
        {
            IdName = idName;
            Fields = new DynamicFields<C>(this);
        }

        internal bool Load(DbDataReader reader, bool throwExceptionOnNotFound = true, bool singleRow = true)
        {
            if (reader.Read())
            {
                id = Helper.GetSafeValue(reader, IdName, 0);

                var dictionary = Dictionary as IDictionary<string, object>;
                var keys = dictionary.Keys.ToList();

                for (int i = 0; i < dictionary.Count; i++)
                {
                    string key = keys[i];

                    if (Types[key].GetInterface("IModl") != null)
                        dictionary[key] = Helper.GetSafeValue(reader, key, dictionary[key], typeof(int?));
                    else
                        dictionary[key] = Helper.GetSafeValue(reader, key, dictionary[key], Types[key]);
                }

                if (singleRow)
                    reader.Close();

                return true;
            }
            else
            {
                reader.Close();

                if (throwExceptionOnNotFound)
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

                    if (!Types.ContainsKey(Fields.LastInsertedMemberName))
                        Types[Fields.LastInsertedMemberName] = property.PropertyType;
                }
            }
        }

        internal void BaseAddSaveFields(Change<C> statement)
        {
            foreach (var field in (IDictionary<string, object>)Dictionary)
            {
                if (Types[field.Key].GetInterface("IModl") != null && !(field.Value is int))
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
                    statement.With(field.Key, field.Value);
            }
        }
    }
}
