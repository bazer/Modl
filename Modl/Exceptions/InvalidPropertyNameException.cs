using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class InvalidPropertyNameException : Exception
    {
        public InvalidPropertyNameException(string message) : base(message)
        {
        }
    }
}
