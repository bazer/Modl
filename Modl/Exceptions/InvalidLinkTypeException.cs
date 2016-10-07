using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class InvalidLinkTypeException : Exception
    {
        public InvalidLinkTypeException(string message) : base(message)
        {
        }
    }
}
