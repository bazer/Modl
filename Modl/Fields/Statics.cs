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
using System.Reflection;

namespace Modl.Fields
{
    internal class Statics<M, IdType> where M : Modl<M, IdType>, new()
    {
        internal static string TableName;
        internal static string IdName;

        private static Dictionary<string, string> Properties = new Dictionary<string, string>();
        private static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        //private static List<PropertyInfo> EmptyProperties = new List<PropertyInfo>();
        private static List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>> EmptyProperties = new List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>>();

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

        internal static void Initialize(Modl<M, IdType> instance)
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
                    instance.Store.LastInsertedMemberName = null;

                    object defaultValue = Helper.GetDefault(property.PropertyType);
                    property.SetValue(instance, defaultValue, null);

                    if (instance.Store.LastInsertedMemberName == null)
                    { 
                        instance.Store.SetValue(property.Name, defaultValue, true);

                        var getDelegate = (Func<M, object>)typeof(Statics<M, IdType>).GetMethod("MakeGetDelegate").MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetGetMethod(true), instance });
                        var setDelegate = (Action<M, object>)typeof(Statics<M, IdType>).GetMethod("MakeSetDelegate").MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetSetMethod(true), instance });
                        EmptyProperties.Add(new Tuple<PropertyInfo, Func<M, object>, Action<M, object>>(property, getDelegate, setDelegate));
                        
                    }
                    
                    SetFieldName(property.Name, instance.Store.LastInsertedMemberName);
                    SetFieldType(instance.Store.LastInsertedMemberName, property.PropertyType);
                }
            }
        }

        internal static void FillFields(Modl<M, IdType> instance)
        {
            foreach (var field in Types)
                instance.Store.SetValue(field.Key, Helper.GetDefault(field.Value));
        }


        internal static void ReadFromEmptyProperties(Modl<M, IdType> instance)
        {
            foreach (var property in EmptyProperties)
                instance.Store.SetValue(property.Item1.Name, property.Item2((M)instance), true);
        }

        internal static void WriteToEmptyProperties(Modl<M, IdType> instance)
        {
            foreach (var property in EmptyProperties)
                property.Item3((M)instance, instance.Store.GetValue<object>(property.Item1.Name));
        }

        //internal static void ReadFromEmptyProperties(Modl<M, IdType> instance)
        //{
        //    foreach (var property in EmptyProperties)
        //        instance.Store.SetValue(property.Name, property.GetValue(instance, null), true);
        //}

        //internal static void WriteToEmptyProperties(Modl<M, IdType> instance)
        //{
        //    foreach (var property in EmptyProperties)
        //        property.SetValue(instance, instance.Store.GetValue<object>(property.Name), null);
        //}

        public static Func<M, object> MakeGetDelegate<T>(MethodInfo @get, Modl<M, IdType> instance)
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, @get);
            return m => f(m);
        }

        public static Action<M, object> MakeSetDelegate<T>(MethodInfo @get, Modl<M, IdType> instance)
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, @get);
            return (m, t) => f(m, (T)t);
        }

    }
}
