using System;
using System.Collections.Generic;
using System.Linq;
using Modl.Metadata;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Modl.Repository;

namespace Modl
{


    

    //public interface IModl
    //{

    //}

    

    //public interface IPropertyChange
    //{
    //    IModl Modl { get; }
    //    IProperty Property { get; }
    //}

    

    //public interface IAggregateChangeModl<T> where T : IModl
    //{
    //    IEnumerable<IModification<T>> Changes { get; }
    //}

    

    public interface IPartialModl<T> where T : IModl
    {
    }

    //public interface IReadModl : IModl
    //{
    //}

    public interface IReadModlWithId<T> where T : IModl, IPartialModl<T>
    {
        Identity Id { get; }
        ICommit Transaction { get; }
    }

    public interface IWriteModl<T> where T : IModl
    {
    }

    //public class ImmutableModl
    //{
    //    public static T Get<T, D>(object id) where T : IReadModl<D>
    //    {
    //        return default(T);
    //    }
    //}

    public class ImmutableModl<M> where M : class, IModl
    {
        static ImmutableModl()
        {
        }

        //public static Definitions Definitions => Handler<M>.Definitions;
        //public static Settings Settings => Handler<M>.Settings;

        //public static bool Exists(object id) => 
        //    List().Any(x => x.Equals(id));

        //public static M Get(object id) => 
        //    Handler<M>.Get(id is Identity ? id as Identity : Identity.FromId(id, Definitions));

        //public static IEnumerable<M> GetAll() => 
        //    Handler<M>.List().Select(id => Get(id));

        //public static IEnumerable<object> List() => 
        //    Handler<M>.List();

        //public static IEnumerable<T> List<T>() => 
        //    Handler<M>.List().Select(id => (T)id);

        //public static string GetName<T>(Expression<Func<T>> e)
        //{
        //    var member = (MemberExpression)e.Body;
        //    return member.Member.Name;
        //}

        public static M Get(object id)
        {
            Func<string> length = () => "test";
            //return DelegateWrapper.WrapAs<M>(length);


            return default(M);
        }

        public static T Get<T>(object id) where T : IPartialModl<M>
        {
            Func<string> length = () => "test";
            //return DelegateWrapper.WrapAs<T>(length);


            return default(T);
        }

        //public static ITransaction Modify<T>(T m, Expression<Func<T, >> e) where T : IReadModl<M>
        //{

        //}

        //public static ITransaction Add<T>(T m) where T : IWriteModl<M>
        //{
        //    return null;
        //    //return (1.0, 1.0);
        //}

        //public static M New() => 
        //    Handler<M>.New();

        //public static M New(object id) => 
        //    Handler<M>.New(id);

        public static IQueryable<M> Query()
        {
            throw new NotImplementedException();

            //var queryParser = QueryParser.CreateDefault();
            //return new Query<M>(queryParser, new QueryExecutor());
        }
    }

    


    public static class ImmutableExtensions
    {
        

        

        //public static Mutation Modify<T>(this IPartialModl<T> m, IWriteModl<T> values) where T : IModl
        //{
        //    return null;
        //}

        //public static Mutation Modify<T, V>(this IPartialModl<T> m, Expression<Func<T, V>> e, V value) where T : IModl
        //{
        //    return null;
        //}

        //public static IEnumerable<IModifyModl<T>> Modify<T, V>(this IReadModl<T> m, Func<T, ((V, V), (V, V))> e) where T : IDefineModl
        //{
        //    return null;
        //}


        //public static Mutation Modify<T, V>(this IEnumerable<IModification> m, Expression<Func<T, V>> e, V value) where T : IMutable
        //{
        //    return null;
        //}

        

        //public static IChangeModl<T> Modify<T, V>(this IChangeModl<T> m, Expression<Func<T, V>> e, V value) where T : IDefineModl
        //{
        //    return null;
        //}
    }
}