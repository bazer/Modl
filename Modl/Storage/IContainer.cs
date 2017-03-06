using System.Collections.Generic;

namespace Modl.Structure.Storage
{
    public interface IContainer
    {
        About About { get; }
        string Hash { get;  }
        Dictionary<string, object> Values { get;  }
        StorageIdentity Identity { get; }
    }
}