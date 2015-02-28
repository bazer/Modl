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
        public Dictionary<string, object> Values { get; set; }
        public ModlIdentity About { get; set; }
        public string Hash { get; set; }

        public ModlStorage()
        {

        }

        public ModlStorage(ModlIdentity identity, Dictionary<string, object> values)
        {
            this.About = identity;
            this.Values = values;
        }
    }
}
