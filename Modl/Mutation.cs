using Modl.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public interface IMutable : IModl
    {
        bool IsModified { get; }
        bool IsNew { get; }
        IEnumerable<IModification> Modifications { get; }
        object this[string property]
        {
            get;
            set;
        }
    }

    public interface IModification
    {
        IMutable Modl { get; }

        //IPropertyChange From { get; }
        IProperty Property { get; }
    }

    public interface IProperty
    {
        string Name { get; }
    }

    public interface ISimpleProperty : IProperty
    {
        object Value { get; }
    }

    public struct SimpleProperty : ISimpleProperty
    {
        public object Value { get; }

        public string Name { get; }

        public SimpleProperty(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public class Modification : IModification
    {
        public IMutable Modl { get; }

        public IProperty Property { get; }

        public Modification(IMutable modl, IProperty property)
        {
            this.Modl = modl;
            this.Property = property;
        }
    }

    public struct Mutation
    {
        public IEnumerable<IModification> Modifications => modifications;
        private List<IModification> modifications;

        public Mutation(IEnumerable<IModification> modifications)
        {
            this.modifications = modifications.ToList();
        }

        public Mutation Concat(IEnumerable<IModification> modifications)
        {
            return new Mutation(this.modifications.Concat(modifications));
        }

        public Mutation Concat(IMutable mutable)
        {
            return Concat(mutable.Modifications);
        }

        public Mutation Concat(Mutation mutation)
        {
            return Concat(mutation.Modifications);
        }
    }

    public static class MutationExtensions
    {
        public static Mutation AppendTo(this Mutation mutation, Mutation mutationToAppendTo)
        {
            return mutationToAppendTo.Concat(mutation);
        }

        public static Mutation AppendTo(this IMutable mutable, Mutation mutationToAppendTo)
        {
            return mutationToAppendTo.Concat(mutable);
        }

        public static Mutation ToMutation(this IMutable mutable)
        {
            return new Mutation(mutable.Modifications);
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
