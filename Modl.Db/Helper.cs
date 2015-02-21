/*
Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

This file is part of Modl.

Modl is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Modl is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Modl.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.ComponentModel;
using System.Data;

namespace Modl.Db
{
    public class Helper
    {
        public static bool IsNumeric(string teststring)
        {
            int i;
            return (Int32.TryParse(teststring, out i));
        }

        public static T GetSafeValue<T>(IDataReader reader, string column)
        {
            return (T)GetSafeValue(reader, column, typeof(T));
        }

        public static object GetSafeValue(IDataReader reader, string column, Type type)
        {
            int c = reader.GetOrdinal(column);

            if (reader[c] == DBNull.Value)
                return GetDefault(type);
            else
                return ConvertTo(type, reader[c]);
        }

        public static T ConvertTo<T>(object value)
        {
            return (T)ConvertTo(typeof(T), value);
        }

        public static object ConvertTo(Type type, object value)
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
