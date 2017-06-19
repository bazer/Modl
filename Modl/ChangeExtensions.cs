using Modl.Instance;
using System;
using System.Collections.Generic;
using Modl.Repository;
using System.Linq.Expressions;

namespace Modl
{

    public static class ChangeExtensions
    {
        public static ChangeCollection AppendTo(this ChangeCollection mutation, ChangeCollection mutationToAppendTo)
        {
            return mutationToAppendTo.Concat(mutation);
        }

        public static ChangeCollection AppendTo(this IMutable mutable, ChangeCollection mutationToAppendTo)
        {
            return mutationToAppendTo.Concat(mutable);
        }

        public static T Mutate<T>(this T m) where T : class, IMutable
        {
            if (m.IsMutable)
                return m;

            return MutableInstanceCreator<T>.NewInstance(m);
            //return Handler<T> m;
        }

        public static T Mutate<T, V>(this T m, Expression<Func<T, V>> property, V value) where T : class, IMutable
        {
            var mut = m.Mutate();

            //var func = e.Compile();
            //func(mut)

            var expression = (MemberExpression)property.Body;
            var name = expression.Member.Name;
            mut[name] = value;

            return mut;
        }

        //public static ChangeCollection GetChanges(this IMutable mutable)
        //{
        //    return new ChangeCollection(mutable.GetChanges());
        //}

        //public static T Mutate<T>(this T m, Func<T, T> e) where T : class, IMutable
        //{
        //    if (m.IsMutable)
        //        return m;

        //    return MutableInstanceCreator<T>.NewInstance(m);
        //}

        //public IEnumerable<IModification> Modifications<T>(this T m) where T : class, IMutable
        //{
        //}

        //public static Mutation ToMutation<T>(this T m) where T : IMutable
        //{
        //    return null;
        //}
    }
}