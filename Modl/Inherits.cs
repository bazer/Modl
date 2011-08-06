using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    public abstract class Inherits<I, M> : Modl<M>
        where I : Modl<I>, new()
        where M : Inherits<I, M>, new()
    {
        public I Parent { get; set; }
    }
}
