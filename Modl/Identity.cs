using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;
using Modl.Structure.Instance;

namespace Modl
{
    public class Identity<M> : IEquatable<Identity<M>>, IComparable<Identity<M>>
        where M : IModl, new()
    {
        public bool IsInternal { get; set; }
        public bool IsAutomatic { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }

        internal M modl { get; set; }
        

        internal Identity(M modl)
        {
            this.modl = modl;

            var definitions = modl.Modl.Backer.Definitions;
            this.IsInternal = !definitions.HasIdProperty;
            this.IsAutomatic = definitions.HasAutomaticId;
            this.Name = IsInternal ? "Id" : definitions.IdProperty.PropertyName;
            this.Type = IsInternal ? typeof(Guid) : definitions.IdProperty.PropertyType;
        }

        public bool IsSet
        {
            get
            {
                modl.Modl.Backer.ReadFromInstanceId(modl);
                return modl.Modl.Backer.HasId();
            }
        }

        public Identity<M> Generate()
        {
            modl.Modl.Backer.GenerateId();
            modl.Modl.Backer.WriteToInstanceId(modl);

            return this;
        }

        public object Get()
        {
            return Get<object>();
        }

        public T Get<T>()
        {
            modl.Modl.Backer.ReadFromInstanceId(modl);

            return (T)modl.Modl.Backer.GetId();
        }

        public Identity<M> Set<T>(T value)
        {
            modl.Modl.Backer.SetId(value, false);
            modl.Modl.Backer.WriteToInstanceId(modl);

            return this;
        }

        public int CompareTo(Identity<M> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Identity<M> other)
        {
            if ((object)other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other.Type != Type)
                return false;

            if (!IsSet || !other.IsSet)
                return false;

            return CheckTypesEquals(Get(), other.Get());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() == typeof(Identity<M>))
                return Equals(obj as Identity<M>);

            if (ReferenceEquals(this, obj))
                return true;

            if (!IsSet)
                return false;

            if (obj.GetType() != Type)
                return false;

            return CheckTypesEquals(Get(), obj);
        }

        private bool CheckTypesEquals(object id, object otherId)
        {
            if (id == null)
                return false;
            else if (Type == typeof(Guid))
                return (Guid)id == (Guid)otherId;
            else if (id is int)
                return (int)id == (int)otherId;
            else if (id is string)
                return (string)id == (string)otherId;
            else
                throw new InvalidIdException("Unsupported Id type");
        }

        public override string ToString()
        {
            return Get().ToString();
        }

        public override int GetHashCode()
        {
            return Get().GetHashCode();
        }

        public static bool operator ==(Identity<M> a, Identity<M> b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Identity<M> a, Identity<M> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Identity<M> a, Guid b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Identity<M> a, Guid b)
        {
            return !(a == b);
        }

        public static bool operator ==(Guid a, Identity<M>  b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator !=(Guid a, Identity<M> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Identity<M> a, int b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Identity<M> a, int b)
        {
            return !(a == b);
        }

        public static bool operator ==(int a, Identity<M> b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator !=(int a, Identity<M> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Identity<M> a, string b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Identity<M> a, string b)
        {
            return !(a == b);
        }

        public static bool operator ==(string a, Identity<M> b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator !=(string a, Identity<M> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Identity<M> a, object b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Identity<M> a, object b)
        {
            return !(a == b);
        }

        public static bool operator ==(object a, Identity<M> b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator !=(object a, Identity<M> b)
        {
            return !(a == b);
        }
    }
}
