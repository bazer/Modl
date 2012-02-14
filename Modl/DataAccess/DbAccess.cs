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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Modl.Query;

namespace Modl.DataAccess
{
	public class DbAccess
	{
        static public bool ExecuteNonQuery(params IQuery[] queries)
		{
            return ExecuteNonQuery(new List<IQuery>(queries));
		}

        static public bool ExecuteNonQuery(List<IQuery> queries)
		{
			return ExecuteNonQuery(Database.GetDbCommands(queries));
		}

		static public bool ExecuteNonQuery(List<IDbCommand> commands)
		{
			for (int i = 0; i < commands.Count; i++)
			{
				if (commands[i].Connection.State != ConnectionState.Open)
					commands[i].Connection.Open();

				commands[i].ExecuteNonQuery();

                if (i + 1 == commands.Count || commands[i].Connection != commands[i + 1].Connection)
                    commands[i].Connection.Close();
			}

            return true;
		}

		static public object ExecuteScalar(Type type, params IQuery[] queries)
		{
			return ExecuteScalar(type, new List<IQuery>(queries));
		}

        static public object ExecuteScalar(Type type, List<IQuery> queries)
		{
            return ExecuteScalar(type, Database.GetDbCommands(queries));
		}

        static public object ExecuteScalar(Type type, List<IDbCommand> commands)
		{
			//T result = default(T);
            object result = null;
			
			for (int i = 0; i < commands.Count; i++)
			{
				if (commands[i].Connection.State != ConnectionState.Open)
					commands[i].Connection.Open();

				object o = commands[i].ExecuteScalar();

                if (i + 1 == commands.Count || commands[i].Connection != commands[i + 1].Connection)
                    commands[i].Connection.Close();

				if (o != null && o != DBNull.Value)
					result = Convert.ChangeType(o, type);
			}

			return result;
		}

        static public IEnumerable<DbDataReader> ExecuteReader(params IQuery[] queries)
        {
            return ExecuteReader(new List<IQuery>(queries));
        }

        static public IEnumerable<DbDataReader> ExecuteReader(List<IQuery> queries)
        {
            return ExecuteReader(Database.GetDbCommands(queries));
        }

        static public IEnumerable<DbDataReader> ExecuteReader(List<IDbCommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Connection.State != ConnectionState.Open)
                    commands[i].Connection.Open();

                yield return (DbDataReader)commands[i].ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        

        //static public DbDataReader ExecuteReader(IDbCommand command)
        //{
        //    if (command.Connection.State != ConnectionState.Open)
        //        command.Connection.Open();

        //    //return (DbDataReader)command.ExecuteReader();
        //    return (DbDataReader)command.ExecuteReader(CommandBehavior.CloseConnection);
        //}
	}
}
