using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Fields
{
    internal class Field
    {
        protected object oldValue;
        protected object newValue;
        protected bool isDirty = false;
        internal bool IsDirty { get { return isDirty; } }
        internal Type Type { get; set; }

        internal Field(object value, Type type)
        {
            oldValue = value;
            newValue = value;

            Type = type;
        }

        internal object Value
        {
            get
            {
                return newValue;
            }
            set
            {
                newValue = value;
                isDirty = oldValue != newValue;
                //isDirty = !EqualityComparer<T>.Default.Equals(oldValue, newValue);
            }
        }

        internal void Reset()
        {
            oldValue = newValue;
            isDirty = false;
        }
    }
}
