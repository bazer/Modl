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
using System.Data.Common;
using Modl.Query;

namespace Modl.Fields
{
    internal class Statics<M> where M : IModl, new()
    {
        internal static List<Table> Tables = new List<Table>();
        //internal static string TableName;
        internal static string IdName { get { return Tables[0].PrimaryKeyName; } }
        internal static Type IdType { get { return Tables[0].PrimaryKeyType; } }
        internal static CacheLevel CacheLevel;
        internal static int CacheTimeout;

        private static Dictionary<string, string> Properties = new Dictionary<string, string>();
        private static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        private static List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>> EmptyProperties = new List<Tuple<PropertyInfo, Func<M, object>, Action<M, object>>>();
        


        private static Database staticDbProvider = null;
        /// <summary>
        /// The default database of this Modl entity. 
        /// This is the same as Config.DefaultDatabase unless a value is specified.
        /// Set to null to clear back to the value of Config.DefaultDatabase.
        /// </summary>
        internal static Database DefaultDatabase
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

        internal static bool IsDirty(IModl instance)
        {
            ReadFromEmptyProperties(instance);
            return GetContents(instance).IsDirty;
        }

        internal static Content GetContents(IModl instance)
        {
            var content = Content.GetContents(instance);

            if (content == null)
                content = AddInstance(instance);

            return content;
        }

        internal static Content AddInstance(IModl instance)
        {
            var content = Content.AddInstance(instance);
            content.Database = DefaultDatabase;
            FillFields(content);

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
            Type instanceType = typeof(M);

            CacheLevel = CacheConfig.DefaultCacheLevel;
            CacheTimeout = CacheConfig.DefaultCacheTimeout;
            //Table.Name = instanceType.Name;

            var content = GetContents(instance);

            ParseClassHierarchy(instanceType, content);

            //Table.IdType = GetFieldType(Table.IdName);
        }

        private static void ParseClassHierarchy(Type type, Content content)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
                ParseClassHierarchy(type.BaseType, content);

            Table table = new Fields.Table();
            table.Name = type.Name;
            table.Type = type;

            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    table.Name = ((NameAttribute)attribute).Name;
                else if (attribute is CacheAttribute)
                {
                    CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                    CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                }
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.CanWrite)
                {
                    string fieldName = property.Name;
                    bool key = false;
                    Type foreignKey = null;

                    foreach (var attribute in property.GetCustomAttributes(false))
                    {
                        if (attribute is NameAttribute)
                            fieldName = ((NameAttribute)attribute).Name;
                        else if (attribute is KeyAttribute)
                            key = true;
                        else if (attribute is ForeignKeyAttribute)
                            foreignKey = ((ForeignKeyAttribute)attribute).Entity;
                        else if (attribute is CacheAttribute)
                        {
                            CacheLevel = ((CacheAttribute)attribute).CacheLevel;
                            CacheTimeout = ((CacheAttribute)attribute).CacheTimeout;
                        }
                    }

                    //content.SetValue(fieldName, Helper.GetDefault(property.PropertyType));

                    var getDelegate = (Func<M, object>)typeof(Statics<M>).GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetGetMethod(true) });
                    var setDelegate = (Action<M, object>)typeof(Statics<M>).GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetSetMethod(true) });
                    EmptyProperties.Add(new Tuple<PropertyInfo, Func<M, object>, Action<M, object>>(property, getDelegate, setDelegate));

                    SetFieldName(property.Name, fieldName);
                    SetFieldType(fieldName, property.PropertyType);


                    table.Fields.Add(fieldName, property.PropertyType);

                    if (key)
                        table.Keys.Add(fieldName, property.PropertyType);

                    if (foreignKey != null)
                        table.ForeignKeys.Add(fieldName, foreignKey);
                }
            }

            Tables.Add(table);
        }

        internal static void FillFields(Content content)
        {
            foreach (var field in Types)
                content.SetValue(field.Key, Helper.GetDefault(field.Value));
        }


        internal static void ReadFromEmptyProperties(IModl instance)
        {
            foreach (var property in EmptyProperties)
                GetContents(instance).SetValue(Properties[property.Item1.Name], property.Item2((M)instance));
        }

        internal static void WriteToEmptyProperties(IModl instance, string propertyName = null)
        {
            foreach (var property in EmptyProperties)
                if (propertyName == null || property.Item1.Name == propertyName)
                    property.Item3((M)instance, GetContents(instance).GetValue<object>(Properties[property.Item1.Name]));
        }

        private static Func<M, object> MakeGetDelegate<T>(MethodInfo method)
        {
            var f = (Func<M, T>)Delegate.CreateDelegate(typeof(Func<M, T>), null, method);
            return m => f(m);
        }

        private static Action<M, object> MakeSetDelegate<T>(MethodInfo method)
        {
            var f = (Action<M, T>)Delegate.CreateDelegate(typeof(Action<M, T>), null, method);
            return (m, t) => f(m, (T)t);
            //return (m, t) => f(m, (T)Convert.ChangeType(t, typeof(T)));
        }

        internal static void Load(IModl instance, DbDataReader reader)
        {
            //id = Helper.GetSafeValue<object>(reader, Statics<M>.IdName);
            
            var content = GetContents(instance);

            foreach (var property in Properties)
            {

                content.SetValue(property.Value, Helper.GetSafeValue(reader, property.Value, GetFieldType(property.Value)));
            }

            content.IsNew = false;
            //var keys = Fields.Keys.ToList();

            //for (int i = 0; i < Fields.Count; i++)
            //{
            //    string key = keys[i];

            //    //if (Fields[key].Type.GetInterface("IModl") != null)
            //    //    SetField(key, Helper.GetSafeValue(reader, key, typeof(int?)));
            //    //else
            //    SetField(key, Helper.GetSafeValue(reader, key, GetFieldType(key)));
            //}

            WriteToEmptyProperties(instance);
        }



        //internal static List<IQuery> GetFields(IModl instance)
        //{
        //    ReadFromEmptyProperties(instance);

        //    var content = GetContents(instance);

        //    List<IQuery> queries = new List<IQuery>();

        //    foreach (var t in Tables)
        //    {
        //        Change query;

        //        if (content.IsNew)
        //            query = new Insert(content.Database, t);
        //        else
        //            query = new Update(content.Database, t).Where(t.Keys.First().Key).EqualTo(content.GetValue<object>(t.Keys.First().Key));

        //        foreach (var fieldName in t.Fields)
        //        {
        //            var field = content.Fields[fieldName];
        //            //if (field.Value.Type.GetInterface("IModl") != null) // && !(field.Value.Value is int))
        //            //{
        //            //    var m = (M)field.Value.Value;

        //            //    if (m == null)
        //            //        yield return new KeyValuePair<string, object>(field.Key, null);
        //            //    //statement.With(field.Key, null);
        //            //    else
        //            //    {
        //            //        if (m.IsDirty())
        //            //            throw new Exception("Child " + m + " is dirty");

        //            //        yield return new KeyValuePair<string, object>(field.Key, m.GetId());
        //            //        //statement.With(field.Key, value.Id);
        //            //    }
        //            //}
        //            if (field.IsDirty && (!content.AutomaticId || fieldName != t.IdName))
        //                query.With(fieldName, field.Value);
        //        }

        //        queries.Add(query);
        //    }

        //    return queries;
        //}
    }
}
