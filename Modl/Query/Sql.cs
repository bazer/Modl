/*
Copyright 2011 Sebastian Öberg (https://github.com/bazer)

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
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Modl.Query
{
    public class Sql
    {
        private StringBuilder builder = new StringBuilder();
        public List<IDataParameter> Parameters = new List<IDataParameter>();
        public string Text { get { return builder.ToString(); } }

        public Sql()
        {
        }

        public Sql(string text, params IDataParameter[] parameters)
        {
            AddText(text);
            AddParameters(parameters);
        }

        public Sql AddParameters(params IDataParameter[] parameters)
        {
            Parameters.AddRange(parameters);

            return this;
        }

        public Sql AddText(string text)
        {
            builder.Append(text);

            return this;
        }

        public Sql AddFormat(string format, params string[] values)
        {
            builder.AppendFormat(format, values);

            return this;
        }

        public Sql Join(string separator, params string[] values)
        {
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                builder.Append(values[i]);

                if (i + 1 < length)
                    builder.Append(separator);
            }

            return this;
        }

        public Sql AddWhereText(string format, params string[] values)
        {
            builder.AppendFormat(format, values);

            return this;
        }
    }
}
