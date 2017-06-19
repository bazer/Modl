using System;
using System.Collections.Generic;
using Modl.Storage;

namespace Modl.Structure.Storage
{
    public interface IContainer
    {
        About About { get; }
        string Hash { get; }
        StorageIdentity Identity { get; }
        Dictionary<string, object> Values { get; }
    }
}