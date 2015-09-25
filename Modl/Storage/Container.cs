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

    public class Container// : IModlStorage
    {
        public Dictionary<string, object> Values { get; set; }
        public About About { get; set; }
        public string Hash { get; set; }
        internal Identity Identity { get; set; }

        public Container()
        {
        }

        public Container(About about, Dictionary<string, object> values)
        {
            this.About = about;
            this.Values = values;
        }
    }
}
