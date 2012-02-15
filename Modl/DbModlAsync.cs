using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Linq;
using Modl.Query;
using System.Linq.Expressions;

namespace Modl
{
    public interface IDbModlAsync<M> : IModl
    {
    }

    public static class DbModlAsync<M>
        where M : IDbModlAsync<M>, new()
    {
        
    }

    public static class DbModlAsyncExtensions
    {
        //public static IdType Id<M, IdType>(this IDbModl dbReader) where IdType : IDbType<IdType>
        //{
        //    //if (IsIdLoaded)
        //    //    return Store.Id;
        //    //else
        //    //    return IdTask.Result;

        //    throw new NotImplementedException();
            
        //}

        

        
    }

    
}
