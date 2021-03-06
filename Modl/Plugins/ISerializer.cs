﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Storage;

namespace Modl.Interfaces
{
    public interface ISerializer: IPipeline
    {
        MemoryStream Serialize(Container storage);
        Container Deserialize(Stream stream);
        object DeserializeObject(object obj, Type toType);
    }
}
