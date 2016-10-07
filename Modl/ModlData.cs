using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Instance;

namespace Modl
{
    public interface IModlData
    {
        Backer Backer { get; set; }
    }

    public class ModlData : IModlData
    {
        public Backer Backer { get; set; }
    }
}
