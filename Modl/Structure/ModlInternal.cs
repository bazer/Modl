using Modl.Cache;
using Modl.Structure;
using Modl.Structure.Metadata;
using Modl.Structure.Storage;
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
        private static Dictionary<string, ModlInstance<M>> Instances { get; set; }

        static ModlInternal()
        {
            Settings = new ModlSettings();
            Metadata = new ModlMetadata<M>();
            Instances = new Dictionary<string, ModlInstance<M>>();
        }

        internal static ModlInstance<M> GetInstance(M m)
        {
            if (m == null)
                throw new NullReferenceException("Modl object is null");

            ModlInstance<M> content;
            if (!Instances.TryGetValue(m.Id, out content))
                throw new Exception("The instance hasn't been attached");

            return content;
        }

        internal static bool HasInstance(M m)
        {
            if (string.IsNullOrWhiteSpace(m.Id))
                throw new Exception("The instance doesn't have a ModlId");

            return Instances.ContainsKey(m.Id);
        }

        internal static void AddInstance(M m)
        {
            if (string.IsNullOrWhiteSpace(m.Id))
                throw new Exception("The instance doesn't have a ModlId");

            if (!HasInstance(m))
                Instances.Add(m.Id, new ModlInstance<M>(m));
        }

        internal static ModlInstance<M> AddFromStorage(IEnumerable<ModlStorage> storage)
        {
            var instance = new M().Modl().GetInstance();
            instance.SetValuesFromStorage(storage);

            return instance;
        }

        internal static bool Delete(M m)
        {
            throw new NotImplementedException();
        }

        internal static M Get(string id)
        {
            //var identity = new ModlAbout
            //{
            //    Id = id.ToString(),
            //    Name = Metadata.ModlName
            //};

            //foreach (var identity in Metadata.GetIdentities(id))
            //{

            //    var stream = Settings.Endpoint.Get(identity);
            //    stream.Position = 0;
            //    var storage = Settings.Serializer.Deserialize(stream);
            //    stream.Dispose();
            //}
            
            if (Instances.ContainsKey(id))
                return Instances[id].Instance;

            var modlInstance = ModlInternal<M>.AddFromStorage(ModlMaterializer.Read(Metadata.GetIdentities(id), Settings).ToList());
            modlInstance.IsNew = false;
            modlInstance.ResetFields();
            modlInstance.WriteToInstance();

            //Statics<M>.WriteToEmptyProperties(m);

            return modlInstance.Instance;
        }


        internal static bool Save(M m)
        {
            var instance = m.GetInstance();

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.Id));

            if (!instance.IsModified)
                return false;

            //object keyValue = null;
            //Type parentType = null;

            //foreach (var t in Metadata.FirstLayer)
            //{
            //    //if (keyValue != null && parentType != null && t.ForeignKeys.Count() != 0)
            //    //{
            //    //    var fk = t.ForeignKeys.Where(x => x.Type == parentType).Select(x => x.Key).SingleOrDefault();

            //    //    if (fk != null)
            //    //        instance.SetValue(fk, keyValue);
            //    //}

            //    //Change query;

            //    //if (content.IsNew)
            //    //    query = new Insert(content.Database, t);
            //    //else
            //    //    query = new Update(content.Database, t).Where(t.PrimaryKeyName).EqualTo(content.GetValue<object>(t.PrimaryKeyName));

            //    //foreach (var f in t.Fields)
            //    //{
            //    //    var field = instance.Properties[f.Key];

            //    //    if (f.Value.GetInterface("IModl") != null && field.Value != null)
            //    //    {
            //    //        var related = field.Value as IModl;

            //    //        if (related.GetInstance().IsModified || related.IsNew())// && saveRelated)
            //    //        {
            //    //            var method = typeof(IModlExtensions).GetMethod("Save");
            //    //            var generic = method.MakeGenericMethod(f.Value);
            //    //            generic.Invoke(null, new object[] { related });

            //    //            //related.Save();
            //    //        }

            //    //        //if (!related.IsNew() && !related.IsDeleted())
            //    //        //    query.With(f.Key, related.GetId());
            //    //    }
            //    //    //else if (field.IsDirty && (!content.AutomaticId || !t.HasKey || f.Key != t.PrimaryKeyName))
            //    //    //    query.With(f.Key, field.Value);
            //    //}



            //    //if (content.IsNew && content.AutomaticId && t.HasKey)
            //    //    keyValue = DbAccess.ExecuteScalar(t.PrimaryKeyType, query, content.Database.GetLastIdQuery());
            //    //else
            //    //    DbAccess.ExecuteScalar(typeof(object), query);

            //    //if (keyValue != null && t.Keys.Count != 0)
            //    //    content.SetValue(t.PrimaryKeyName, keyValue);

            //    parentType = t.Type;
            //}

            //foreach (var storage in instance.GetStorage())
            //{
            //    var stream = Modl<M>.Settings.Serializer.Serialize(storage);
            //    stream.Position = 0;

            //    Modl<M>.Settings.Endpoint.Save(storage.About, stream);

            //    stream.Dispose();
            //}

            ModlMaterializer.Write(instance.GetStorage(), Settings);

            instance.IsNew = false;
            instance.ResetFields();
            //instance.WriteToInstance();

            return true;
        }
    }
}
