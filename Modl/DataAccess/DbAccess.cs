using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace Modl.DataAccess
{
    public class DbAccess
    {
        static public int ExecuteNonQuery(params IQuery[] statements)
        {
            return ExecuteNonQuery(new List<IQuery>(statements));
        }

        static public int ExecuteNonQuery(List<IQuery> statements)
        {
            if (statements.Count == 0)
                return 0;

            return ExecuteNonQuery(Helper.ToSqlCommand(statements));
        }

        static public int ExecuteNonQuery(IDbCommand command)
        {
            using (IDbConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                int result = command.ExecuteNonQuery();
                connection.Close();
                return result;
            }
        }

        static public T ExecuteScalar<T>(params IQuery[] statements)
        {
            return ExecuteScalar<T>(new List<IQuery>(statements));
        }

        static public T ExecuteScalar<T>(List<IQuery> statements)
        {
            return ExecuteScalar<T>(Helper.ToSqlCommand(statements));
        }

        static public T ExecuteScalar<T>(IDbCommand command)
		{
            using (IDbConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
				T result = (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
                connection.Close();
				return result;
            }
		}

        static public IDataReader ExecuteReader(params IQuery[] statements)
        {
            return ExecuteReader(new List<IQuery>(statements));
        }

        static public IDataReader ExecuteReader(List<IQuery> statements)
        {
            return ExecuteReader(Helper.ToSqlCommand(statements));
        }

        static public IDataReader ExecuteReader(IDbCommand command)
        {
            IDbConnection connection = new SqlConnection(Config.ConnectionString);
            command.Connection = connection;
            connection.Open();
            
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
