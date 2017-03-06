using Modl.Instance;
using Modl.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Modl
{
    public interface IMutation
    {
        Guid Id { get; }
        IMutable Modl { get; }
        IProperty OldProperty { get; }
        IProperty NewProperty { get; }
    }

    public interface IMutable : IModl
    {
        bool IsModified { get; }
        bool IsNew { get; }
        IEnumerable<IMutation> Modifications { get; }
        object this[string property] { get; set; }
    }

    public interface IProperty
    {
        string Name { get; }
        PropertyDefinition Metadata { get; }
    }

    public interface ISimpleProperty : IProperty
    {
        object Value { get; }
    }

    public interface IRelationProperty : IProperty
    {
        IModl Value { get; }
        Identity Id { get; }
    }

    public interface IMutationCollection
    {
        IEnumerable<IMutation> Modifications { get; }

        MutationCollection Concat(IEnumerable<IMutation> modifications);
        MutationCollection Concat(IMutable mutable);
        MutationCollection Concat(MutationCollection mutation);
    }

    public struct MutationCollection : IMutationCollection
    {
        private List<IMutation> modifications;

        public MutationCollection(IEnumerable<IMutation> modifications)
        {
            this.modifications = modifications.ToList();
        }

        public IEnumerable<IMutation> Modifications => modifications.AsEnumerable();

        public MutationCollection Concat(IEnumerable<IMutation> modifications)
        {
            return new MutationCollection(this.modifications.Concat(modifications));
        }

        public MutationCollection Concat(IMutable mutable)
        {
            return Concat(mutable.Modifications);
        }

        public MutationCollection Concat(MutationCollection mutation)
        {
            return Concat(mutation.Modifications);
        }
    }

    public struct SimpleProperty : ISimpleProperty
    {
        public SimpleProperty(PropertyDefinition metadata, string name, object value)
        {
            this.Metadata = metadata;
            this.Name = name;
            this.Value = value;
        }

        public PropertyDefinition Metadata { get; }
        public string Name { get; }
        public object Value { get; }
    }

    public struct RelationProperty : IRelationProperty
    {
        public RelationProperty(PropertyDefinition metadata, string name, IModl value)
        {
            this.Metadata = metadata;
            this.Name = name;
            this.Value = value;
            this.Id = value?.Id();
        }

        public PropertyDefinition Metadata { get; }
        public string Name { get; }
        public Identity Id { get; }
        public IModl Value { get; }
    }

    public static class MutationExtensions
    {
        public static MutationCollection AppendTo(this MutationCollection mutation, MutationCollection mutationToAppendTo)
        {
            return mutationToAppendTo.Concat(mutation);
        }

        public static MutationCollection AppendTo(this IMutable mutable, MutationCollection mutationToAppendTo)
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

        public static MutationCollection ToMutation(this IMutable mutable)
        {
            return new MutationCollection(mutable.Modifications);
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

    public class Modification : IMutation
    {
        public Modification(Guid id, IMutable modl, IProperty oldProperty, IProperty newProperty)
        {
            this.Id = id;
            this.Modl = modl;
            this.OldProperty = oldProperty;
            this.NewProperty = newProperty;
        }

        public IMutable Modl { get; }

        public IProperty OldProperty { get; }
        public IProperty NewProperty { get; }

        public Guid Id { get; }
    }
}