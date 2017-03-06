using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Storage
{
    //public interface IModlStorage
    //{

    //}

    public class Container : IContainer// : IModlStorage
    {
        public Dictionary<string, object> Values { get; }
        public About About { get; }
        public string Hash { get; }
        public StorageIdentity Identity { get; }

        public Container()
        {
        }

        public Container(StorageIdentity identity, About about, Dictionary<string, object> values)
        {
            this.Identity = identity;
            this.About = about;
            this.Values = values;
        }
    }
}
