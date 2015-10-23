using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Modl.Linq
{
    public class Query<M> : QueryableBase<M>
        where M : IModl, new()
    {
        public Query(IQueryParser queryParser, IQueryExecutor executor)
            : base(new DefaultQueryProvider(typeof(Query<>), queryParser, executor))
        {
        }

        public Query(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }


    //public class Query<M> : IQueryable<M>
    //    where M : IModl, new()
    //{
    //    public Type ElementType
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public Expression Expression
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public IQueryProvider Provider
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public IEnumerator<M> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
