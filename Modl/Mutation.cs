using Modl.Instance;
using Modl.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using Modl.Repository;

namespace Modl
{
    public interface IValue
    {
        string Hash { get; }
        object Content { get; }
        Type Type { get; }
    }

    public struct Value : IValue
    {
        public string Hash { get; }
        public object Content { get; }
        public Type Type { get; }

        public Value(string hash, object content, Type type)
        {
            Hash = hash;
            Content = content;
            Type = type;
        }

        public Value(object content)
        {
            Content = content;
            Type = content?.GetType();
            Hash = null;
        }
    }

    public interface IMutable : IModl
    {
        bool IsModified { get; }
        bool IsNew { get; }
        bool IsCommited { get; }
        ChangeCollection GetChanges();
        object this[string property] { get; set; }
    }

    public interface IProperty
    {
        Guid Id { get; }
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
        Identity RelationId { get; }
    }



    public struct SimpleProperty : ISimpleProperty
    {
        public SimpleProperty(PropertyDefinition metadata, string name, object value)
        {
            this.Metadata = metadata;
            this.Name = name;
            this.Value = value;
            this.Id = Guid.Empty;
        }

        public PropertyDefinition Metadata { get; }
        public string Name { get; }
        public object Value { get; }

        public Guid Id { get; }
    }

    public struct RelationProperty : IRelationProperty
    {
        public RelationProperty(PropertyDefinition metadata, string name, IModl value)
        {
            this.Metadata = metadata;
            this.Name = name;
            this.Value = value;
            this.RelationId = value?.Id();
            this.Id = Guid.Empty;
        }

        public PropertyDefinition Metadata { get; }
        public string Name { get; }
        public IModl Value { get; }

        public Identity RelationId { get; }

        public Guid Id { get; }
    }


}