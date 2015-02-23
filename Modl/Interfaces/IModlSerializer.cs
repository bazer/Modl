using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Interfaces
{
    public interface IModlSerializer: IModlPipeline
    {
        MemoryStream Serialize(ModlStorage storage);
        ModlStorage Deserialize(Stream stream);
    }
}
