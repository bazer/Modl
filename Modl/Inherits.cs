using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    public abstract class Inherits<I, M, IdType> : Modl<M, IdType>
        where I : Modl<I, IdType>, new()
        where M : Inherits<I, M, IdType>, new()
    {
        public I Parent { get; set; }
    }
}
