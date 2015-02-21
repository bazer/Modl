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
        public static ModlSettings Settings { get; private set; }
        public static ModlMetadata<M> Metadata { get; private set; }

        static ModlInternal()
        {
            Settings = new ModlSettings();
            Metadata = new ModlMetadata<M>();
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
            return GetData(instance).IsModified;
        }

        internal static ModlData GetData(IModl instance)
        {
            var content = ModlData.GetContents(instance);

            if (content == null)
                content = AddInstance(instance);

            return content;
        }

        internal static void SetId(IModl instance, object value)
        {
            var content = GetData(instance);
            content.SetValue<object>(Metadata.IdName, value);
            content.AutomaticId = false;

            WriteToEmptyProperties(instance, Metadata.IdName);
        }

        internal static object GetId(IModl instance)
        {
            return GetData(instance).GetValue<object>(Metadata.IdName);
        }

        internal static ModlIdentity GetIdentity(IModl instance)
        {
            return new ModlIdentity
            {
                Id = GetData(instance).GetValue<object>(Metadata.IdName).ToString(),
                ModlName = Metadata.ModlName
            };
        }

        

        

       

        internal static void FillFields(ModlData content)
        {
            foreach (var field in Metadata.Types)
                content.SetValue(field.Key, GetDefault(field.Value));
        }


        internal static void ReadFromEmptyProperties(IModl instance)
        {
            foreach (var property in Metadata.EmptyProperties)
                GetData(instance).SetValue(Metadata.Properties[property.Item1.Name], property.Item2((M)instance));
        }

        internal static void WriteToEmptyProperties(IModl instance, string propertyName = null)
        {
            foreach (var property in Metadata.EmptyProperties)
                if (propertyName == null || property.Item1.Name == propertyName)
                    property.Item3((M)instance, GetData(instance).GetValue<object>(Metadata.Properties[property.Item1.Name]));
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

        private static object GetDefault(Type type)
        {
            if (!type.IsValueType)
                return null;
            else
                return Activator.CreateInstance(type);
        }
    }
}
