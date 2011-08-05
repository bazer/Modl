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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Modl.DataAccess
{
    public class DbTransaction : IDisposable
    {
        //protected string DatabaseName;

        //public DbTransaction(string databaseName)
        //{
        //    DatabaseName = databaseName;
        //}

        //private bool isTransactionPending = false;
        //public bool IsTransactionPending
        //{
        //    get { return isTransactionPending; }
        //}

        //private IDbConnection dbConnection;
        //private IDbTransaction dbTransaction;
        //private IDbConnection DbConnection
        //{
        //    get
        //    {
        //        if (dbConnection == null || dbTransaction == null || isTransactionPending == false)
        //        {
        //            isTransactionPending = true;
        //            dbConnection = Config.GetConnection(DatabaseName);
        //            dbConnection.Open();
        //            dbTransaction = dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        //        }

        //        return dbConnection;
        //    }
        //}

        //public void Commit()
        //{
        //    if (isTransactionPending)
        //    {
        //        dbTransaction.Commit();
        //        isTransactionPending = false;
        //    }

        //    Dispose();
        //}

        //public void Rollback()
        //{
        //    if (isTransactionPending)
        //    {
        //        isTransactionPending = false;
        //        if (dbTransaction != null)
        //            dbTransaction.Rollback();
        //    }

        //    Dispose();
        //}

        //public int ExecuteNonQuery(params IQuery[] statements)
        //{
        //    return ExecuteNonQuery(new List<IQuery>(statements));
        //}

        //public int ExecuteNonQuery(List<IQuery> statements)
        //{
        //    if (statements.Count == 0)
        //        return 0;

        //    return ExecuteNonQuery(Helper.ToSqlCommand(statements));
        //}

        //public int ExecuteNonQuery(IDbCommand cmd)
        //{
        //    try
        //    {
        //        cmd.Connection = DbConnection;
        //        cmd.Transaction = dbTransaction;
        //        return cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception)
        //    {
        //        Rollback();
        //        throw;
        //    }
        //}

        //public static int ExecuteNonQueryWithCommit(string databaseName, List<IQuery> statements)
        //{
        //    DbTransaction dbTrans = new DbTransaction(databaseName);
        //    int result = dbTrans.ExecuteNonQuery(statements);
        //    dbTrans.Commit();
        //    return result;
        //}

        //public T ExecuteScalar<T>(params IQuery[] statements)
        //{
        //    return ExecuteScalar<T>(new List<IQuery>(statements));
        //}

        //public T ExecuteScalar<T>(List<IQuery> statements)
        //{
        //    return ExecuteScalar<T>(Helper.ToSqlCommand(statements));
        //}

        //public T ExecuteScalar<T>(IDbCommand command)
        //{
        //    try
        //    {
        //        command.Connection = DbConnection;
        //        command.Transaction = dbTransaction;
        //        object result = command.ExecuteScalar();
        //        return (T)Convert.ChangeType(result, typeof(T));
        //    }
        //    catch (Exception)
        //    {
        //        Rollback();
        //        throw;
        //    }
        //}

        //public IDataReader ExecuteReader(params IQuery[] statements)
        //{
        //    return ExecuteReader(new List<IQuery>(statements));
        //}

        //public IDataReader ExecuteReader(List<IQuery> statements)
        //{
        //    return ExecuteReader(Helper.ToSqlCommand(statements));
        //}

        ///// <summary>
        ///// Close this reader when done! (or use a using-statement)
        ///// </summary>
        ///// <param name="cmd"></param>
        ///// <returns></returns>
        //public IDataReader ExecuteReader(IDbCommand cmd)
        //{
        //    try
        //    {
        //        cmd.Connection = DbConnection;
        //        cmd.Transaction = dbTransaction;
        //        return cmd.ExecuteReader();
        //    }
        //    catch (Exception)
        //    {
        //        Rollback();
        //        throw;
        //    }
        //}

        //private void Close()
        //{
        //    if (isTransactionPending == true)
        //        throw (new Exception("TransactionalUpdate: Transaction pending, cannot close!"));

        //    if (dbConnection != null)
        //        dbConnection.Close();
        //}

        #region IDisposable Members

        public void Dispose()
        {
            //Close();

            //if (dbConnection != null)
            //    dbConnection.Dispose();

            //if (dbTransaction != null)
            //    dbTransaction.Dispose();
        }

        #endregion
    }
}
