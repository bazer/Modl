using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Modl.Attributes;

namespace Modl.Fields
{
    internal class Statics<M> where M : Modl<M>, new()
    {
        internal static string TableName;
        internal static string IdName;

        private static Dictionary<string, string> Properties = new Dictionary<string, string>();
        private static Dictionary<string, Type> Types = new Dictionary<string, Type>();

        internal static void SetFieldName(string propertyName, string fieldName)
        {
            Properties[propertyName] = fieldName;
        }

        internal static string GetFieldName(string propertyName)
        {
            return Properties[propertyName];
        }

        internal static void SetFieldType(string fieldName, Type type)
        {
            Types[fieldName] = type;
        }

        internal static Type GetFieldType(string fieldName)
        {
            return Types[fieldName];
        }

        internal static void Initialize(Modl<M> instance)
        {
            foreach (var attribute in typeof(M).GetCustomAttributes(true))
            {
                if (attribute is TableAttribute)
                    TableName = ((TableAttribute)attribute).Name;
                else if (attribute is IdAttribute)
                    IdName = ((IdAttribute)attribute).Name;
            }

            if (string.IsNullOrEmpty(TableName))
                TableName = typeof(M).Name;

            if (string.IsNullOrEmpty(IdName))
                IdName = "Id";


            foreach (PropertyInfo property in typeof(M).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(instance, Helper.GetDefault(property.PropertyType), null);
                    SetFieldName(property.Name, instance.Store.LastInsertedMemberName);
                    SetFieldType(instance.Store.LastInsertedMemberName, property.PropertyType);
                }
            }
        }

        internal static void FillFields(Modl<M> instance)
        {
            foreach (var field in Types)
                instance.Store.SetValue(field.Key, Helper.GetDefault(field.Value));
        }
    }
}
