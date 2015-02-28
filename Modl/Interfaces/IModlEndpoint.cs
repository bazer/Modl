using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Interfaces
{
    public interface IModlEndpoint //<M> : IModlPipeline<M>
        //where M : IModl
    {
        Stream Get(ModlIdentity identity);
        void Save(ModlIdentity identity, MemoryStream stream);
         
    }
}
