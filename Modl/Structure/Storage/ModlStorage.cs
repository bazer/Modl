using Modl.Structure;
using Modl.Structure.Storage;
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
        public ModlAbout About { get; set; }
        public string Hash { get; set; }
        internal ModlIdentity Identity { get; set; }

        public ModlStorage()
        {
        }

        public ModlStorage(ModlAbout about, Dictionary<string, object> values)
        {
            this.About = about;
            this.Values = values;
        }

        
    }
}
