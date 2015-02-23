using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public interface IModlStorage
    {

    }

    public class ModlStorage : IModlStorage
    {
        public ModlIdentity Identity { get; set; }
        public Dictionary<string, object> Values { get; set; }

        public ModlStorage()
        {

        }

        public ModlStorage(ModlIdentity identity, Dictionary<string, object> values)
        {
            this.Identity = identity;
            this.Values = values;
        }
    }
}
