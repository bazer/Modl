using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Instance
{
    public interface IValue
    {
        bool IsModified { get; }
        //void Set(object value);
        object Get();
        void Reset();
    }
}
