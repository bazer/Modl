using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Instance;

namespace Modl
{
    public class Link<M>: MultiLink<M>
        where M : IModl, new()
    {
        internal Link(string name, IModl m) : base(name, m)
        {
        }

        public M Val
        {
            get
            {
                return this.FirstOrDefault();
            }
            set
            {
                this.Add(value);
            }
        }
            
        
    }
}
