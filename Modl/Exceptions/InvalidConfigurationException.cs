using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(string message) : base(message)
        {
        }
    }
}
