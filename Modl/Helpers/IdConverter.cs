using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;

namespace Modl.Helpers
{
    public class IdConverter
    {
        public static bool HasValue(object id)
        {
            if (id == null)
                return false;
            else if (id is Guid)
                return id != null && (Guid)id != Guid.Empty;
            else if (id is int)
                return (int)id != 0;
            else if (id is string)
                return !string.IsNullOrWhiteSpace(id as string);
            else if (id.GetType().IsValueType)
                return id != Activator.CreateInstance(id.GetType());
            else
                return true;
        }

        public static bool AreEqual(Type type, object a, object b)
        {
            if (a == null && b == null)
                return true;
            else if (a == null)
                return false;
            else if (b == null)
                return false;

            if (a.GetType() != b.GetType())
                return false;
            else if (a.GetType() != type)
                return false;
            else if (b.GetType() != type)
                return false;

            return a.Equals(b);
        }

        public static object Convert(object id, bool allowCasting, Type idType)
        {
            if (id is Identity)
                id = (id as Identity).Get();

            if (idType == typeof(Guid))
                return ConvertToGuid(id, allowCasting);

            if (idType == typeof(int))
                return ConvertToInt32(id, allowCasting);

            if (idType == typeof(string))
                return ConvertToString(id, allowCasting);

            if (idType != id.GetType())
                throw new InvalidIdException($"Id value should be of type {idType}, but is of type {id.GetType()}");
            else
                throw new InvalidIdException($"Id value is of unsupported type {id.GetType()}");
        }

        public static Guid ConvertToGuid(object id, bool allowCasting = true)
        {
            Guid guidValue;

            if (id is Guid)
            {
                guidValue = (Guid)id;
            }
            else if (!allowCasting)
            {
                throw new InvalidIdException("Id is not a Guid");
            }
            else
            {
                if (!(id is string))
                    throw new InvalidIdException("Id is not a string or Guid");

                if (!Guid.TryParse(id as string, out guidValue))
                    throw new InvalidIdException("Id is not convertable to a Guid");
            }

            return guidValue;
        }

        public static int ConvertToInt32(object id, bool allowCasting = true)
        {
            int intValue;

            if (id is int)
            {
                intValue = (int)id;
            }
            else if (!allowCasting)
            {
                throw new InvalidIdException("Id is not a int");
            }
            else
            {
                if (!(id is string) && !(id is short) && !(id is long))
                    throw new InvalidIdException("Id is not a int, short, long or string");

                if (!int.TryParse(id.ToString(), out intValue))
                    throw new InvalidIdException("Id is not convertable to a int");
            }

            return intValue;
        }

        public static string ConvertToString(object id, bool allowCasting = true)
        {
            if (id is string)
                return id as string;
            else if (allowCasting)
                return id.ToString();
            else
                throw new InvalidIdException("Id is not a string");
        }
    }
}
