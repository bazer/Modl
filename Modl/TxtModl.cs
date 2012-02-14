using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Linq;
using Modl.Query;
using System.Linq.Expressions;

namespace Modl
{
    public interface ITxtModl<M> : IModl
    {
    }

    public static class TxtModl<M>
        where M : ITxtModl<M>, new()
    {
    }

    public static class ITxtModlExtensions
    {
    }
}
