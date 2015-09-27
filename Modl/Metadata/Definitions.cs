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
    public class Definitions//<M>
        //where M : IModl, new()
    {
        static Dictionary<Type, Definitions> AllDefinitions = new Dictionary<Type, Definitions>();

        private Layer FirstLayer { get; set; }// = new Layer(typeof(M));
        public bool HasPrimaryKey => FirstLayer.HasPrimaryKey;
        public bool HasAutomaticKey => FirstLayer.HasAutomaticKey;
        public Property PrimaryKey => FirstLayer.PrimaryKey;
        public List<Property> Properties => FirstLayer.AllProperties;

        public static Definitions Get(Type type)
        {
            if (!AllDefinitions.ContainsKey(type))
                AllDefinitions.Add(type, new Definitions(type));

            return AllDefinitions[type];
        }

        public Definitions(Type type)
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

        internal IEnumerable<Container> GetStorage(Backer instance)
        {
            return FirstLayer.GetStorage(instance);
        }

        internal void SetValuesFromStorage(Backer instance, IEnumerable<Storage.Container> storage)
        {
            FirstLayer.SetValuesFromStorage(instance, storage);
        }
    }
}
