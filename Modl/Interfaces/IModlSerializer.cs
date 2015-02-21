using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Interfaces
{
    public interface IModlSerializer: IModlPipeline
        //<M> : IModlPipeline<M>
        // where M : IModl
    {
        M ConvertFrom<M>(Stream stream) where M: IModl, new();
        MemoryStream ConvertTo<M>(M modl) where M: IModl;
    }
}
