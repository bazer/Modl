using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl
{
    public interface IModlData
    {
        PropertyValues PropertyValues { get; set; }
    }

    public class ModlData : IModlData
    {
        public PropertyValues PropertyValues { get; set; }
    }
}
