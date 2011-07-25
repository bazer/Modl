using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Modl.Fields
{
    internal class Statics<C> where C : Modl<C>, new()
    {
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

        internal static void Initialize(Modl<C> instance)
        {
            foreach (PropertyInfo property in typeof(C).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(instance, Helper.GetDefault(property.PropertyType), null);
                    //Fields[LastInsertedMemberName].Type = property.PropertyType;
                    Statics<C>.SetFieldName(property.Name, instance.Store.LastInsertedMemberName);
                    Statics<C>.SetFieldType(instance.Store.LastInsertedMemberName, property.PropertyType);
                }
            }
        }

        internal static void FillFields(Modl<C> instance)
        {
            foreach (var field in Types)
            {
                instance.Store.SetValue(field.Key, Helper.GetDefault(field.Value));
            }
        }
    }
}
