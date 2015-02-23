﻿using Modl.Cache;
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
        private static Dictionary<int, ModlInstance<M>> Instances { get; set; }

        static ModlInternal()
        {
            Settings = new ModlSettings();
            Metadata = new ModlMetadata<M>();
            Instances = new Dictionary<int, ModlInstance<M>>();
        }

        internal static ModlInstance<M> GetInstance(M m)
        {
            if (m == null)
                throw new NullReferenceException("Modl object is null");

            ModlInstance<M> content;
            if (!Instances.TryGetValue(m.GetHashCode(), out content))
                throw new Exception("The instance hasn't been attached");

            return content;
        }

        internal static bool HasInstance(M m)
        {
            return Instances.ContainsKey(m.GetHashCode());
        }

        internal static void AddInstance(M m)
        {
            if (!HasInstance(m))
                Instances.Add(m.GetHashCode(), new ModlInstance<M>(m));
        }

        internal static ModlInstance<M> AddFromStorage(ModlStorage storage)
        {
            var instance = new M().Modl().GetInstance();
            instance.SetValuesFromStorage(storage);

            return instance;
        }

        internal static bool Delete(M m)
        {
            throw new NotImplementedException();
        }

        internal static bool Save(M m)
        {
            var instance = m.GetInstance();

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (!instance.IsModified)
                return false;

            object keyValue = null;
            Type parentType = null;

            foreach (var t in Metadata.Layers)
            {
                if (keyValue != null && parentType != null && t.ForeignKeys.Count != 0)
                {
                    var fk = t.ForeignKeys.Where(x => x.Value == parentType).Select(x => x.Key).SingleOrDefault();

                    if (fk != null)
                        instance.SetValue(fk, keyValue);
                }

                //Change query;

                //if (content.IsNew)
                //    query = new Insert(content.Database, t);
                //else
                //    query = new Update(content.Database, t).Where(t.PrimaryKeyName).EqualTo(content.GetValue<object>(t.PrimaryKeyName));

                //foreach (var f in t.Fields)
                //{
                //    var field = instance.Properties[f.Key];

                //    if (f.Value.GetInterface("IModl") != null && field.Value != null)
                //    {
                //        var related = field.Value as IModl;

                //        if (related.GetInstance().IsModified || related.IsNew())// && saveRelated)
                //        {
                //            var method = typeof(IModlExtensions).GetMethod("Save");
                //            var generic = method.MakeGenericMethod(f.Value);
                //            generic.Invoke(null, new object[] { related });

                //            //related.Save();
                //        }

                //        //if (!related.IsNew() && !related.IsDeleted())
                //        //    query.With(f.Key, related.GetId());
                //    }
                //    //else if (field.IsDirty && (!content.AutomaticId || !t.HasKey || f.Key != t.PrimaryKeyName))
                //    //    query.With(f.Key, field.Value);
                //}



                //if (content.IsNew && content.AutomaticId && t.HasKey)
                //    keyValue = DbAccess.ExecuteScalar(t.PrimaryKeyType, query, content.Database.GetLastIdQuery());
                //else
                //    DbAccess.ExecuteScalar(typeof(object), query);

                //if (keyValue != null && t.Keys.Count != 0)
                //    content.SetValue(t.PrimaryKeyName, keyValue);

                parentType = t.Type;
            }

            
            var stream = Modl<M>.Settings.Serializer.Serialize(instance.GetStorage());
            stream.Position = 0;

            Modl<M>.Settings.Endpoint.Save(instance.GetIdentity(), stream);

            stream.Dispose();


            instance.IsNew = false;
            instance.ResetFields();
            instance.WriteToEmptyProperties();

            return true;
        }
    }
}
