using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl
{
    public class ModlData : IModlData
        //where M : IModl, new()
    {
        //public string Id { get; set; }
        public InstanceData Instance { get; set; }
    }
}
