using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Linq;
using Modl.Query;
using System.Linq.Expressions;

namespace Modl
{
    public interface IMvcModl<M> : IModl
    {
    }

    public static class MvcModl<M>
        where M : IMvcModl<M>, new()
    {
    }

    public static class IMvcModlExtensions
    {
    }
}
