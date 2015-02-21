using Modl.Cache;
using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure
{
    internal class ModlInternal<M>
        where M : IModl, new()
    {
        
        //internal static string TableName;
        //public static string IdName { get { return Tables.Last().PrimaryKeyName; } }
        //internal static Type IdType { get { return Tables.Last().PrimaryKeyType; } }
        //internal static string TableName { get { return Tables.Last().Name; } }
        internal static CacheLevel CacheLevel;
        internal static int CacheTimeout;

        

        public static ModlSettings Settings { get; private set; }
        public static ModlMetadata<M> Metadata { get; private set; }

        static ModlInternal()
        {
            Type instanceType = typeof(M);

            CacheLevel = CacheConfig.DefaultCacheLevel;
            CacheTimeout = CacheConfig.DefaultCacheTimeout;

            Settings = new ModlSettings();
            Metadata = new ModlMetadata<M>();
            //Table.Name = instanceType.Name;

            //var content = GetContents(instance);

            ParseClassHierarchy(instanceType);

            //Table.IdType = GetFieldType(Table.IdName);
        }

        public static ModlData AddInstance(IModl instance)
        {
            //var content = GetContents(instance);
            ModlData content;

            if (!ModlData.HasInstance(instance))
            {
                content = ModlData.AddInstance(instance);
                FillFields(content);
            }
            else
                content = ModlData.GetContents(instance);



            return content;
        }

        internal static bool IsModified(IModl instance)
        {
            ReadFromEmptyProperties(instance);
            return GetContents(instance).IsModified;
        }

        internal static ModlData GetContents(IModl instance)
        {
            var content = ModlData.GetContents(instance);

            if (content == null)
                content = AddInstance(instance);

            return content;
        }

        internal static void SetId(IModl instance, object value)
        {
            var content = GetContents(instance);
            content.SetValue<object>(Metadata.IdName, value);
            content.AutomaticId = false;

            WriteToEmptyProperties(instance, Metadata.IdName);
        }

        internal static object GetId(IModl instance)
        {
            return GetContents(instance).GetValue<object>(Metadata.IdName);
        }

        internal static ModlIdentity GetIdentity(IModl instance)
        {
            return new ModlIdentity
            {
                Id = GetContents(instance).GetValue<object>(Metadata.IdName).ToString(),
                ModlName = Metadata.ModlName
            };
        }

        

        

        private static void ParseClassHierarchy(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
                ParseClassHierarchy(type.BaseType);

            ModlLayer layer = new Structure.ModlLayer();
            layer.Name = type.Name;
            layer.Type = type;

            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is NameAttribute)
                    layer.Name = ((NameAttribute)attribute).Name;
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

                    var getDelegate = (Func<M, object>)typeof(ModlInternal<M>).GetMethod("MakeGetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetGetMethod(true) });
                    var setDelegate = (Action<M, object>)typeof(ModlInternal<M>).GetMethod("MakeSetDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property.GetSetMethod(true) });
                    Metadata.EmptyProperties.Add(new Tuple<PropertyInfo, Func<M, object>, Action<M, object>>(property, getDelegate, setDelegate));

                    Metadata.SetFieldName(property.Name, fieldName);
                    Metadata.SetFieldType(fieldName, property.PropertyType);


                    layer.Fields.Add(fieldName, property.PropertyType);

                    if (key)
                        layer.Keys.Add(fieldName, property.PropertyType);

                    if (foreignKey != null)
                        layer.ForeignKeys.Add(fieldName, foreignKey);
                }
            }

            Metadata.Layers.Insert(0, layer);
        }

        internal static void FillFields(ModlData content)
        {
            foreach (var field in Metadata.Types)
                content.SetValue(field.Key, Helper.GetDefault(field.Value));
        }


        internal static void ReadFromEmptyProperties(IModl instance)
        {
            foreach (var property in Metadata.EmptyProperties)
                GetContents(instance).SetValue(Metadata.Properties[property.Item1.Name], property.Item2((M)instance));
        }

        internal static void WriteToEmptyProperties(IModl instance, string propertyName = null)
        {
            foreach (var property in Metadata.EmptyProperties)
                if (propertyName == null || property.Item1.Name == propertyName)
                    property.Item3((M)instance, GetContents(instance).GetValue<object>(Metadata.Properties[property.Item1.Name]));
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

        public static bool Save(M m)
        {
            var content = m.GetContent();
            ReadFromEmptyProperties(m);

            if (content.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (!IsModified(m))
                return false;

            object keyValue = null;
            Type parentType = null;

            foreach (var t in Metadata.Layers)
            {
                if (keyValue != null && parentType != null && t.ForeignKeys.Count != 0)
                {
                    var fk = t.ForeignKeys.Where(x => x.Value == parentType).Select(x => x.Key).SingleOrDefault();

                    if (fk != null)
                        content.SetValue(fk, keyValue);
                }

                //Change query;

                //if (content.IsNew)
                //    query = new Insert(content.Database, t);
                //else
                //    query = new Update(content.Database, t).Where(t.PrimaryKeyName).EqualTo(content.GetValue<object>(t.PrimaryKeyName));

                foreach (var f in t.Fields)
                {
                    var field = content.Fields[f.Key];

                    if (f.Value.GetInterface("IModl") != null && field.Value != null)
                    {
                        var related = field.Value as IModl;

                        if (related.GetContent().IsModified || related.IsNew())// && saveRelated)
                        {
                            var method = typeof(IModlExtensions).GetMethod("Save");
                            var generic = method.MakeGenericMethod(f.Value);
                            generic.Invoke(null, new object[] { related });

                            //related.Save();
                        }

                        //if (!related.IsNew() && !related.IsDeleted())
                        //    query.With(f.Key, related.GetId());
                    }
                    //else if (field.IsDirty && (!content.AutomaticId || !t.HasKey || f.Key != t.PrimaryKeyName))
                    //    query.With(f.Key, field.Value);
                }



                //if (content.IsNew && content.AutomaticId && t.HasKey)
                //    keyValue = DbAccess.ExecuteScalar(t.PrimaryKeyType, query, content.Database.GetLastIdQuery());
                //else
                //    DbAccess.ExecuteScalar(typeof(object), query);

                //if (keyValue != null && t.Keys.Count != 0)
                //    content.SetValue(t.PrimaryKeyName, keyValue);

                parentType = t.Type;
            }





            var settings = Modl<M>.Settings;

            var stream = settings.Serializer.ConvertTo(m);
            stream.Position = 0;

            settings.Endpoint.Save(GetIdentity(m), stream);

            stream.Dispose();


            content.IsNew = false;
            content.ResetFields();

            WriteToEmptyProperties(m);

            return true;
        }
    }
}
