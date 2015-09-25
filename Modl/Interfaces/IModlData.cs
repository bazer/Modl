using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl
{
    public interface IModlData//<M>
        //where M : IModl, new()
    {
        //string Id { get; set; }
        InstanceData Instance { get; set; }
    }
}
