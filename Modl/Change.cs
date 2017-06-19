using System;

namespace Modl
{
    public enum ChangeType
    {
        Created,
        Deleted,
        Value
    }

    public interface IChange
    {
        Guid Id { get; }
        bool IsCommited { get; }
        IMutable Modl { get; }
        IValue NewValue { get; }
        IValue OldValue { get; }
        IProperty Property { get; }
        ChangeType Type { get; }
    }

    public class Change : IChange
    {
        public Change(Guid id, IMutable modl, IProperty oldProperty, IProperty newProperty, ChangeType type)
        {
            this.Id = id;
            this.Modl = modl;
            this.Property = newProperty;
            this.OldValue = GetValue(oldProperty);
            this.NewValue = GetValue(newProperty);
            this.Type = type;
        }

        private IValue GetValue(IProperty property)
        {
            switch (property)
            {
                case null: return null;
                case ISimpleProperty p: return new Value(p.Value);
                case IRelationProperty p: return new Value(p.RelationId?.Get());
                default: throw new NotImplementedException();
            }
        }

        public Guid Id { get; }

        public bool IsCommited { get; private set; }

        public IMutable Modl { get; }

        public IValue NewValue { get; }
        public IValue OldValue { get; }
        public IProperty Property { get; }

        public ChangeType Type { get; }

        public void SetCommited()
        {
            IsCommited = true;
        }
    }

    //public interface IValueChange : IChange
    //{
    //    IProperty NewProperty { get; }
    //    IProperty OldProperty { get; }
    //}

    //public class ValueChange : IValueChange
    //{
    //    public Change(Guid id, IMutable modl, IProperty oldProperty, IProperty newProperty, ChangeType type)
    //    {
    //        this.Id = id;
    //        this.Modl = modl;
    //        this.OldProperty = oldProperty;
    //        this.NewProperty = newProperty;
    //        this.Type = type;
    //    }

    //    public Guid Id { get; }

    //    public bool IsCommited { get; private set; }

    //    public IMutable Modl { get; }

    //    public IProperty NewProperty { get; }

    //    public IProperty OldProperty { get; }

    //    public ChangeType Type { get; }

    //    public void SetCommited()
    //    {
    //        IsCommited = true;
    //    }
    //}
}