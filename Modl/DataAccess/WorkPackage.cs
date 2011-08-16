using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Query;
using System.Threading.Tasks;

namespace Modl.DataAccess
{
    internal enum WorkType
    {
        Read,
        Write,
        Scalar,
        Mixed
    }

    internal interface IWorkPackage
    {
        WorkType Type { get; }
        int ParameterCount { get; }
        void SetResult(object result);
        IQuery[] GetWork();
        void DoWork();
    }

    internal class WorkPackage<T> : IWorkPackage
    {

        private IQuery[] queries;
        private T result;
        private TaskCompletionSource<T> task;

        public WorkType Type { get; private set; }
        public bool HasResult { get; private set; }

        public int ParameterCount
        {
            get { return queries.Sum(x => x.ParameterCount); }
        }

        public WorkPackage(WorkType type, params IQuery[] queries)
        {
            this.Type = type;
            this.queries = queries;
        }

        public void SetResult(object result)
        {
            try
            {
                this.result = (T)result;

                if (task != null)
                    task.SetResult(this.result);

                HasResult = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
                task.SetException(e);
            }
        }

        public Task<T> GetResultAsync()
        {
            if (task == null)
            {
                task = new TaskCompletionSource<T>();

                if (HasResult)
                    task.SetResult(result);
            }

            return task.Task;
        }


        public IQuery[] GetWork()
        {
            return queries;
        }

        public Task<T> DoWorkAsync()
        {
            return Task<T>.Factory.StartNew(() =>
            {
                DoWork();
                return result;
            });
        }

        public void DoWork()
        {
            try
            {
                if (Type == WorkType.Write)
                    DbAccess.ExecuteNonQuery(GetWork());
                else if (Type == WorkType.Scalar)
                    SetResult(DbAccess.ExecuteScalar<T>(GetWork()));
                else if (Type == WorkType.Read)
                    SetResult(DbAccess.ExecuteReader(GetWork()).First());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);

                if (task != null)
                    task.SetException(e);
                else
                    throw;
            }
        }
    }
}
