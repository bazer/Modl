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
        IEnumerable<StorageIdentity> List(StorageIdentity identity);
        Stream Get(StorageIdentity identity);
        void Save(StorageIdentity identity, MemoryStream stream);
        void Delete(StorageIdentity identity);
    }
}
