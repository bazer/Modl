using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;

namespace Modl
{
    internal class Helper
    {
        public static bool IsNumeric(string teststring)
        {
            int i;
            return (Int32.TryParse(teststring, out i));
        }

        public static T GetSafeValue<T>(IDataReader reader, string column, T defaultValue)
        {
            return (T)GetSafeValue(reader, column, defaultValue, typeof(T));
        }

        public static dynamic GetSafeValue(IDataReader reader, string column, dynamic defaultValue, Type type)
        {
            int c = reader.GetOrdinal(column);

            if (reader[c] == DBNull.Value)
                return defaultValue;
            else
                return ConvertTo(type, reader[c]);
        }

        public static T ConvertTo<T>(object value)
        {
            return (T)ConvertTo(typeof(T), value);
        }

        public static dynamic ConvertTo(Type type, object value)
        {
            if (value == null)
                return GetDefault(type);

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                type = new NullableConverter(type).UnderlyingType;

            if (type.IsEnum)
                return Enum.ToObject(type, value);
            else
                return Convert.ChangeType(value, type);
        }

        public static object GetDefault(Type type)
        {
            if (!type.IsValueType)
                return null;
            else
                return Activator.CreateInstance(type);
        }

        
    }
}
