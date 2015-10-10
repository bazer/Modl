using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Structure.Instance
{
    public interface IValue
    {
        bool IsModified { get; }
        void Set(object value);
        object Get();
        void Reset();
    }

    public class SimpleValue : IValue
    {
        public virtual bool IsModified { get; private set; }

        protected object oldValue;
        protected object newValue;

        public SimpleValue(object value)
        {
            oldValue = value;
            newValue = value;
        }

        public void Set(object value)
        {
            newValue = value;
            IsModified = !object.Equals(oldValue, newValue);
        }

        public object Get()
        {
            return newValue;
        }

        public void Reset()
        {
            oldValue = newValue;
            IsModified = false;
        }
    }
}
