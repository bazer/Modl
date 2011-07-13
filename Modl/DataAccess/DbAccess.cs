using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using Modl.DatabaseProviders;

namespace Modl.DataAccess
{
    public class DbAccess
    {
        static public void ExecuteNonQuery(params IQuery[] queries)
        {
            ExecuteNonQuery(new List<IQuery>(queries));
        }

        static public void ExecuteNonQuery(List<IQuery> queries)
        {
            ExecuteNonQuery(Database.GetDbCommands(queries));
        }

        static public void ExecuteNonQuery(List<IDbCommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Connection.State != ConnectionState.Open)
                    commands[i].Connection.Open();

                commands[i].ExecuteNonQuery();

                if (i + 1 == commands.Count || commands[i].Connection != commands[i + 1].Connection)
                    commands[i].Connection.Close();
            }
        }

        static public T ExecuteScalar<T>(params IQuery[] queries)
        {
            return ExecuteScalar<T>(new List<IQuery>(queries));
        }

        static public T ExecuteScalar<T>(List<IQuery> queries)
        {
            return ExecuteScalar<T>( Database.GetDbCommands(queries));
        }

        static public T ExecuteScalar<T>(List<IDbCommand> commands)
		{
            T result = default(T);
            
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Connection.State != ConnectionState.Open)
                    commands[i].Connection.Open();

                object o = commands[i].ExecuteScalar();

                if (i+1 == commands.Count || commands[i].Connection != commands[i+1].Connection)
                    commands[i].Connection.Close();

                if (o != null)
                    result = (T)Convert.ChangeType(o, typeof(T));
            }

            return result;
		}

        static public DbDataReader ExecuteReader(IQuery query)
        {
            return ExecuteReader(query.ToDbCommand());
        }

        static public DbDataReader ExecuteReader(IDbCommand command)
        {
            command.Connection.Open();
            
            return (DbDataReader)command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
