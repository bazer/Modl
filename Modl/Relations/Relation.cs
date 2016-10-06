using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;

namespace Modl.Relations
{
    public class Relation<M> where M : IModl, new()
    {
        public string Name { get; internal set; }
        public Backer Backer { get; internal set; }

        public T GetValue<T>()
            where T : IModl, new()
        {
            return Backer.GetRelationValue<T>(Name);
        }

        public void SetValue<T>(T value)
            where T : IModl, new()
        {
            Backer.SetRelationValue(Name, value);
        }
    }
}
