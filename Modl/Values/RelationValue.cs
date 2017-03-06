using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Exceptions;

namespace Modl.Instance
{
    public class RelationValue : IValue
    {
        private List<Identity> newIdValues { get; set; } = new List<Identity>();
        private List<Identity> oldIdValues { get; set; } = new List<Identity>();

        public bool IsModified
        {
            get
            {
                //var firstNotSecond = newIdValues.Except(oldIdValues).ToList();
                //var secondNotFirst = oldIdValues.Except(newIdValues).ToList();

                return newIdValues.Except(oldIdValues).Any() || oldIdValues.Except(newIdValues).Any();
            }
        }
        public RelationValue()
        {
        }

        public RelationValue(IEnumerable<Identity> idValues)
        {
            this.newIdValues = idValues
                //.Select(id => new RelationIdValue(id))
                .ToList();

            Reset();
        }

        public void Add(Identity id)
        {
            if (Has(id))
                throw new InvalidIdException($"Id '{id}' already exists in relation.");

            this.newIdValues.Add(id);
        }

        public bool Has(Identity id) => this.newIdValues.Contains(id);

        public object Get()
        {
            return All().Select(x => x.Get());
        }

        public IEnumerable<Identity> All()
        {
            return newIdValues.AsEnumerable();
        }

        public void Set(IEnumerable<Identity> idValues)
        {
            this.newIdValues = idValues.ToList();
        }

        public void Set(Identity id)
        {
            this.newIdValues = new List<Identity> { id };
        }

        public void Reset()
        {
            this.oldIdValues = this.newIdValues.ToList();
        }

        //public override bool IsModified
        //{
        //    get
        //    {
        //        return base.IsModified;
        //    }
        //}
    }

    public struct RelationIdValue : IEquatable<RelationIdValue>
    {
        public object Id { get; }
        //public bool IsLoaded { get; set; }

        public RelationIdValue(object id)
        {
            this.Id = id;
        }

        public bool Equals(RelationIdValue other) => other.Id.Equals(Id);

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
