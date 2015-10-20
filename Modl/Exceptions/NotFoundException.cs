using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
