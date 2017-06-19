using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class NotMutableException : Exception
    {
        public NotMutableException(string message) : base(message)
        {
        }
    }
}
