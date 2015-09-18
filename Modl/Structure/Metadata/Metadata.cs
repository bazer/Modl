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
    public class Metadata<M>
        where M : IModl, new()
    {
        private Layer<M> FirstLayer { get; set; }
        public bool HasPrimaryKey => FirstLayer.HasPrimaryKey;
        public Property<M> PrimaryKey => FirstLayer.PrimaryKey;
        public List<Property<M>> Properties => FirstLayer.AllProperties;

        public Metadata()
        {
            FirstLayer = new Layer<M>(typeof(M));
        }

        internal IEnumerable<Identity> GetIdentities(object id)
        {
            return FirstLayer.GetIdentities(id);
        }

        internal IEnumerable<Storage.Storage> GetStorage(Instance<M> instance)
        {
            return FirstLayer.GetStorage(instance);
        }

        internal void SetValuesFromStorage(Instance<M> instance, IEnumerable<Storage.Storage> storage)
        {
            FirstLayer.SetValuesFromStorage(instance, storage);
        }

        
    }
}
