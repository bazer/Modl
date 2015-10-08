using Modl.Cache;
using Modl.Structure.Metadata;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl
{
    internal class Handler<M> where M : IModl, new()
    {
        public static Settings Settings { get { return Settings.Get(typeof(M)); } }
        public static Definitions Definitions { get { return Definitions.Get(typeof(M)); } }

        static Handler()
        {
        }
        
        internal static M InitializeModl(M m)
        {
            if (m.ModlData == null)
            {
                m.ModlData = new ModlData
                {
                    Backer = new Backer(typeof(M))
                };

                if (!m.ModlData.Backer.HasId() && Definitions.HasAutomaticKey)
                    m.ModlData.Backer.GenerateId(m);
            }

            return m;
        }

        internal static M AddFromStorage(IEnumerable<Container> storage)
        {
            var m = New(storage.First().About.Id);
            m.ModlData.Backer.SetValuesFromStorage(storage);
            m.ModlData.Backer.ResetValuesToUnmodified();
            m.ModlData.Backer.WriteToInstance(m);
            m.ModlData.Backer.IsNew = false;

            return m;
        }

        internal static M New()
        {
            return new M().Modl();
        }

        internal static M New(object id)
        {
            return New().SetId(id);
        }

        internal static M Get(object id)
        {
            return AddFromStorage(Materializer.Read(Definitions.GetIdentities(id), Settings).ToList());
        }

        internal static bool Save(M m, bool includeRelations)
        {
            var backer = m.ModlData.Backer;

            if (backer.IsDeleted)
                throw new Exception(string.Format("Trying to save a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (includeRelations)
            {
                foreach (var property in Definitions.Properties.Where(x => x.IsRelation))
                {
                    typeof(Backer)
                        .GetMethod("SaveRelation", BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(property.PropertyType)
                        .Invoke(backer, new object[] { property });
                }
            }


            if (!backer.IsModified(m))
                return false;

            if (!backer.HasId() && Definitions.HasAutomaticKey)
                backer.GenerateId(m);
            else if (!backer.HasId())
                throw new Exception($"Id not set. Class: {typeof(M)}");

            Materializer.Write(backer.GetStorage(), Settings.Get(typeof(M)));

            backer.IsNew = false;
            backer.ResetValuesToUnmodified();

            return true;
        }

        

        internal static bool Delete(M m)
        {
            var instance = m.ModlData.Backer;

            if (instance.IsNew)
                throw new Exception(string.Format("Trying to delete a new object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            if (instance.IsDeleted)
                throw new Exception(string.Format("Trying to delete a deleted object. Class: {0}, Id: {1}", typeof(M), m.GetId()));

            Materializer.Delete(instance.GetStorage(), Settings.Get(typeof(M)));
            instance.IsDeleted = true;

            return true;
        }
    }
}
