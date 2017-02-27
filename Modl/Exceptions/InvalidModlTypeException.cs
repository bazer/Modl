using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class InvalidModlTypeException : Exception
    {
        public InvalidModlTypeException(string message) : base(message)
        {
        }
    }
}
