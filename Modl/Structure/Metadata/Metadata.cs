using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl.Structure.Metadata
{
    public class Metadata//<M>
        //where M : IModl, new()
    {
        static Dictionary<Type, Metadata> AllMetadata = new Dictionary<Type, Metadata>();

        private Layer FirstLayer { get; set; }// = new Layer(typeof(M));
        public bool HasPrimaryKey => FirstLayer.HasPrimaryKey;
        public Property PrimaryKey => FirstLayer.PrimaryKey;
        public List<Property> Properties => FirstLayer.AllProperties;

        public static Metadata Get(Type type)
        {
            if (!AllMetadata.ContainsKey(type))
                AllMetadata.Add(type, new Metadata(type));

            return AllMetadata[type];
        }

        public Metadata(Type type)
        {
            FirstLayer = new Layer(type);
        }

        //static Metadata()
        //{
            
        //}

        internal IEnumerable<Identity> GetIdentities(object id)
        {
            return FirstLayer.GetIdentities(id);
        }

        internal IEnumerable<Storage.Storage> GetStorage(InstanceData instance)
        {
            return FirstLayer.GetStorage(instance);
        }

        internal void SetValuesFromStorage(InstanceData instance, IEnumerable<Storage.Storage> storage)
        {
            FirstLayer.SetValuesFromStorage(instance, storage);
        }
    }
}
