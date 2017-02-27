using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Modl.Linq;
//using Modl.Query;
using System.Linq.Expressions;

namespace Modl.Mvc
{
    public interface IMvcModl : IModl
    {
    }

    public class MvcModl<M> : Modl<M>
        where M : class, IMvcModl
    {
    }

    public static class IMvcModlExtensions
    {
    }
}
