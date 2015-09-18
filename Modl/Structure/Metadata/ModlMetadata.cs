using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Metadata
{
    public class ModlMetadata<M>
        where M : IModl, new()
    {
        private ModlLayer<M> FirstLayer { get; set; }
        public bool HasPrimaryKey => FirstLayer.HasPrimaryKey;
        public ModlProperty<M> PrimaryKey => FirstLayer.PrimaryKey;
        public List<ModlProperty<M>> Properties => FirstLayer.AllProperties;

        public ModlMetadata()
        {
            FirstLayer = new ModlLayer<M>(typeof(M));
        }

        internal IEnumerable<ModlIdentity> GetIdentities(object id)
        {
            return FirstLayer.GetIdentities(id);
        }

        internal IEnumerable<ModlStorage> GetStorage(ModlInstance<M> instance)
        {
            return FirstLayer.GetStorage(instance);
        }

        internal void SetValuesFromStorage(ModlInstance<M> instance, IEnumerable<ModlStorage> storage)
        {
            FirstLayer.SetValuesFromStorage(instance, storage);
        }

        
    }
}
