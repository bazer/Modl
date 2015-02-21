using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl.Db.Query;
using System.Threading.Tasks;

namespace Modl.Db.DataAccess
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
        object DoWork();
        string GetDebugSql();
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

        public Task<T> DoWorkAsync()
        {
            return Task<T>.Factory.StartNew(() =>
            {
                DoWork();
                return result;
            });
        }

        public Task<T> AwaitResult()
        {
            if (task == null)
            {
                task = new TaskCompletionSource<T>();

                if (HasResult)
                    task.TrySetResult(result);
            }

            return task.Task;
        }

        public void SetResult(object result)
        {
            try
            {
                this.result = (T)result;

                if (task != null)
                    task.TrySetResult(this.result);

                HasResult = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
                
                if (task != null)
                    task.SetException(e);
            }
        }

        public object DoWork()
        {
            try
            {
                object r = null;

                if (Type == WorkType.Write)
                    r = DbAccess.ExecuteNonQuery(GetWork());
                else if (Type == WorkType.Scalar)
                    r = DbAccess.ExecuteScalar(typeof(T), GetWork());
                else if (Type == WorkType.Read)
                    r = DbAccess.ExecuteReader(GetWork()).First();

                SetResult(r);

                return r;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);

                if (task != null)
                    task.SetException(e);
                else
                    throw;
            }

            return null;
        }

        public IQuery[] GetWork()
        {
            return queries;
        }

        public string GetDebugSql()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var query in GetWork())
                sb.AppendLine(query.ToString());

            return sb.ToString();
        }
    }
}
