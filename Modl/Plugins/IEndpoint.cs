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
    public interface IEndpoint
    {
        IEnumerable<Identity> List(Identity identity);
        Stream Get(Identity identity);
        void Save(Identity identity, MemoryStream stream);
        void Delete(Identity identity);
    }
}
