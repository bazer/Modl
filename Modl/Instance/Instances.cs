using System;
using System.Collections.Generic;
using System.Linq;

namespace Modl.Instance
{
    public class Instances<M>
         where M : class, IModl
    {
        private List<WeakReference> References { get; } = new List<WeakReference>();

        internal void AddInstance(M modl)
        {
            References.Add(new WeakReference(modl));
        }

        internal void CullInstances()
        {
            References.RemoveAll(x => x.Target == null);
        }

        internal IEnumerable<M> GetInstances()
        {
            CullInstances();

            return References
                .Select(x => x.Target)
                .Where(x => x != null)
                .Select(x => (M)x);
        }
    }
}