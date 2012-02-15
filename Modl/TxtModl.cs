using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Linq;
using Modl.Query;
using System.Linq.Expressions;
using Modl.Fields;

namespace Modl
{
    public interface ITxtModl : IModl
    {
    }

    public class TxtModl<M> : Modl<M>
        where M : ITxtModl, new()
    {
    }

    public static class ITxtModlExtensions
    {
        public static bool IsNewText<M>(this M m) where M : ITxtModl, new()
        {
            return m.GetContent().IsNew;
        }
    }
}
