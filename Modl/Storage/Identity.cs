﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Storage
{
    public class StorageIdentity
    {
        public object Id { get; set; }
        public Type IdType { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}
