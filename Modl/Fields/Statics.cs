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
using System.Reflection;
using Modl.Cache;

namespace Modl.Fields
{
    internal class Statics<M> where M : IModl, new()
    {
        internal static string TableName;
        internal static string IdName;
        internal static Type IdType;
        internal static CacheLevel CacheLevel;
        internal static int CacheTimeout;

        private static Dictionary<string, string> Properties = new Dictionary<string, string>();
        private static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        //private static List<PropertyInfo> EmptyProperties = new List<PropertyInfo>();
        private static List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>> EmptyProperties = new List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>>();

        private static Dictionary<int, Content<M>> Contents = new Dictionary<int, Content<M>>();

        private static Database staticDbProvider = null;


        /// <summary>
        /// The default database of this Modl entity. 
        /// This is the same as Config.DefaultDatabase unless a value is specified.
        /// Set to null to clear back to the value of Config.DefaultDatabase.
        /// </summary>
        public static Database DefaultDatabase
        {
            get
            {
                if (staticDbProvider == null)
                    return Config.DefaultDatabase;

                return staticDbProvider;
            }
            set
            {
                staticDbProvider = value;
            }
        }

        static Statics()
        {
            Initialize(new M());
        }

        internal static Content<M> AddInstance(IModl instance)
        {
            var content = new Content<M>(instance);
            FillFields(content);
            Contents.Add(instance.GetHashCode(), content);

            return content;
        }

        internal static Content<M> GetContents(IModl instance)
        {
            Content<M> content;
            if (!Contents.TryGetValue(instance.GetHashCode(), out content))
                content = AddInstance(instance);

            return content;
        }

        internal static void SetId(IModl instance, object value)
        {
            var content = GetContents(instance);
            content.SetValue<object>(IdName, value);
            content.AutomaticId = false;

            WriteToEmptyProperties(instance, IdName);
        }

        internal static object GetId(IModl instance)
        {
            return GetContents(instance).GetValue<object>(IdName);
        }

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

        internal static void Initialize(IModl instance)
        {
            CacheLevel = CacheConfig.DefaultCacheLevel;
            CacheTimeout = CacheConfig.DefaultCacheTimeout;

            foreach (var attribute in typeof(M).GetCustomAttributes(true))
            {
                if (attribute is TableAttribute)
                    TableName = ((TableAttribute)attribute).Name;
                else if (attribute is IdAttribute)
                    IdName = ((IdAttribute)attribute).Name;
                else if (attribute is CacheAttribute)
                {
                    CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                    CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                }
            }

            if (string.IsNullOrEmpty(TableName))
                TableName = typeof(M).Name;

            if (string.IsNullOrEmpty(IdName))
                IdName = "Id";

            var content = GetContents(instance);
            foreach (PropertyInfo property in typeof(M).GetProperties())
            {
                if (property.CanWrite)
                {
                    content.LastInsertedMemberName = null;

                    object defaultValue = Helper.GetDefault(property.PropertyType);
                    property.SetValue(instance, defaultValue, null);

                    if (content.LastInsertedMemberName == null)
                    {
                        content.SetValue(property.Name, defaultValue, true);

                        var getDelegate = (Func<M, object>)typeof(Statics<M>).GetMethod("MakeGetDelegate").MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetGetMethod(true) });
                        var setDelegate = (Action<M, object>)typeof(Statics<M>).GetMethod("MakeSetDelegate").MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetSetMethod(true) });
                        EmptyProperties.Add(new Tuple<PropertyInfo, Func<M, object>, Action<M, object>>(property, getDelegate, setDelegate));
                        
                    }

                    SetFieldName(property.Name, content.LastInsertedMemberName);
                    SetFieldType(content.LastInsertedMemberName, property.PropertyType);
                }
            }

            IdType = GetFieldType(IdName);
        }

        internal static void FillFields(Content<M> content)
        {
            foreach (var field in Types)
                content.SetValue(field.Key, Helper.GetDefault(field.Value));
        }


        internal static void ReadFromEmptyProperties(IModl instance)
        {
            foreach (var property in EmptyProperties)
                GetContents(instance).SetValue(property.Item1.Name, property.Item2((M)instance), true);
        }

        internal static void WriteToEmptyProperties(IModl instance, string propertyName = null)
        {
            foreach (var property in EmptyProperties)
                if (propertyName == null || property.Item1.Name == propertyName)
                    property.Item3((M)instance, GetContents(instance).GetValue<object>(property.Item1.Name));
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

        public static Func<M, object> MakeGetDelegate<T>(MethodInfo method)
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        public static Action<M, object> MakeSetDelegate<T>(MethodInfo method)
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, method);
            return (m, t) => f(m, (T)t);
            //return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        }

    }
}
