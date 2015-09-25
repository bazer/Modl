using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public interface IModl
    {
        IModlData ModlData { get; set; }
    }

    //public interface IModl<M>
    //    where M : object, new()
    //{
    //    IModlData<M> ModlData { get; set; }
    //    //string GetId();
    //    //void SetId(string id);
    //}
}
