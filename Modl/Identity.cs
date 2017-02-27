using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;
using Modl.Helpers;
using Modl.Instance;
using Modl.Metadata;

namespace Modl
{
    //public class Identity
    //{

    //}

    public class Identity : IEquatable<Identity>, IComparable<Identity>
        //where M : class, IModl, new()
    {
        //internal Identity(M modl)
        //{
        //    //this.modl = modl;

        //    //var definitions = modl.Modl.Backer.Definitions;
            
        //}

        private Identity(object idValue, Definitions definitions, bool isNewId = false)
        {
            //this.IdValue = idValue;
            this.Definitions = definitions;
            this.IsNewId = isNewId;

            if (Definitions.HasIdProperty && !IsNewId)
                this.IdValue = IdConverter.Convert(idValue, true, Definitions.IdProperty.PropertyType);
            else
                this.IdValue = IdConverter.ConvertToGuid(idValue);
        }

        //private void Init()
        //{
        //    //this.IsInternal = !definitions.HasIdProperty;
        //    //this.IsAutomatic = definitions.HasAutomaticId;
        //    //this.Name = IsInternal ? "Id" : definitions.IdProperty.PropertyName;
        //    //this.Type = IsInternal ? typeof(Guid) : definitions.IdProperty.PropertyType;
        //}

        //public static Identity FromModl(M modl)
        //{
        //    return new Identity(modl.Modl.Backer.Getid())
        //}

        public static Identity FromId(object id, Definitions definitions) => new Identity(id, definitions);
        public static Identity FromNewId(object id, Definitions definitions) => new Identity(id, definitions, true);

        public static Identity GenerateNewId(Definitions definitions)
        {
            if (definitions.HasAutomaticId && definitions.HasIdProperty && definitions.IdProperty.PropertyType != typeof(Guid))
                throw new InvalidIdException("Only automatic Id of type Guid is supported");

            return new Identity(Guid.NewGuid(), definitions, true);
        }

        private readonly object IdValue;
        private readonly bool IsNewId;
        //private readonly Backer Backer;
        private readonly Definitions Definitions;

        //private Definitions Definitions => Backer.Definitions;
        public bool IsAutomatic => Definitions.HasAutomaticId;
        public bool IsInternal => !Definitions.HasIdProperty;


        public bool IsSet
        {
            get
            {
                return !IsNewId;
                //modl.Modl.Backer.ReadFromInstanceId(modl);
                //return modl.Modl.Backer.HasId();
            }
        }

        public string Name => IsInternal ? "Id" : Definitions.IdProperty.PropertyName;
        public Type Type => IsInternal ? typeof(Guid) : Definitions.IdProperty.PropertyType;

        //internal M modl { get; set; }


        public static bool operator !=(Identity a, Identity b)
        {
            return !(a == b);
        }

        public static bool operator !=(Identity a, Guid b)
        {
            return !(a == b);
        }

        public static bool operator !=(Guid a, Identity b)
        {
            return !(a == b);
        }

        public static bool operator !=(Identity a, int b)
        {
            return !(a == b);
        }

        public static bool operator !=(int a, Identity b)
        {
            return !(a == b);
        }

        public static bool operator !=(Identity a, string b)
        {
            return !(a == b);
        }

        public static bool operator !=(string a, Identity b)
        {
            return !(a == b);
        }

        public static bool operator !=(Identity a, object b)
        {
            return !(a == b);
        }

        public static bool operator !=(object a, Identity b)
        {
            return !(a == b);
        }

        public static bool operator ==(Identity a, Identity b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.Equals(b);
        }

        public static bool operator ==(Identity a, Guid b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator ==(Guid a, Identity b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator ==(Identity a, int b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator ==(int a, Identity b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator ==(Identity a, string b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator ==(string a, Identity b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        public static bool operator ==(Identity a, object b)
        {
            if ((object)a == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator ==(object a, Identity b)
        {
            if ((object)b == null)
                return false;

            return b.Equals(a);
        }

        //public static Identity ReadId(M m)
        //{
        //    if (m.IsInitialized())
        //        return m.Id();

        //    var backer = new Backer(typeof(M));
        //    backer.ReadFromInstanceId(m);

        //    if (!backer.HasId())
        //    {
        //        if (Handler<M>.Definitions.HasAutomaticId)
        //            backer.GenerateId();
        //        else
        //            backer.GenerateInternalId();
        //    }
        //}
        public int CompareTo(Identity other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Identity other)
        {
            if ((object)other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other.Type != Type)
                return false;

            //if (!IsSet || !other.IsSet)
            //    return false;

            return CheckTypesEquals(Get(), other.Get());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() == typeof(Identity))
                return Equals(obj as Identity);

            if (ReferenceEquals(this, obj))
                return true;

            //if (!IsSet)
            //    return false;

            if (obj.GetType() != Type)
                return false;

            return CheckTypesEquals(Get(), obj);
        }

        //public Identity Generate()
        //{
        //    Backer.GenerateId();
        //    //modl.Modl.Backer.WriteToInstanceId(modl);

        //    return this;
        //}

        public object Get()
        {
            return Get<object>();
        }

        public T Get<T>()
        {
            return (T)IdValue;
            //modl.Modl.Backer.ReadFromInstanceId(modl);

            //return (T)modl.Modl.Backer.GetId();
        }

        public override int GetHashCode()
        {
            return IdValue.GetHashCode();
        }

        //public Identity Set<T>(T value)
        //{
            
        //    //Backer.SetId(value, false);
        //    //modl.Modl.Backer.WriteToInstanceId(modl);

        //    return this;
        //}
        public override string ToString()
        {
            return Get().ToString();
        }

        private bool CheckTypesEquals(object id, object otherId)
        {
            if (id == null)
                return false;
            else if (id is Guid && otherId is Guid)//  Type == typeof(Guid))
                return (Guid)id == (Guid)otherId;
            else if (id is int && otherId is int)
                return (int)id == (int)otherId;
            else if (id is string && otherId is string)
                return (string)id == (string)otherId;
            else
                return false;
        }
    }
}
