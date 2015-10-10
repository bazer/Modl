using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Exceptions
{
    public class RecordNotFoundException : ApplicationException
    {
        public RecordNotFoundException() : base() { }
        public RecordNotFoundException(string message) : base(message) { }
        public RecordNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        protected RecordNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
