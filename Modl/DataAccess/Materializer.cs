using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Modl.Fields;
using System.Threading.Tasks;

namespace Modl.DataAccess
{
    public class Materializer<M> : IDisposable
        where M : IDbModl, new()
    {
        private DbDataReader reader;
        private Database database;

        public bool IsDone { get { return reader.IsClosed; } }
        //public bool IsDone { get; private set; }

        private object id;
        private bool isIdSet = false;

        public static Task<Materializer<M>> Async(Task<DbDataReader> reader, Database database)
        {
            return reader.ContinueWith(r =>
            {
                return new Materializer<M>(r.Result, database);
            });
        }

        public Materializer(DbDataReader reader, Database database)
        {
            this.reader = reader;
            this.database = database;

            StepReader();
        }


        public object Peak()
        {
            if (IsDone)
                throw new Exception("Reader is closed");

            if (!isIdSet)
            {
                id = Helper.GetSafeValue<object>(reader, Statics<M>.IdName);
                isIdSet = true;
            }

            return id;
        }

        public M Read()
        {
            if (IsDone)
                throw new Exception("Reader is closed");

            //if (IsDone)
            //    return null;
            try
            {
                return DbModl<M>.Load(reader, database);

                //var m = DbModl<M>.New(database);
                //m.GetContent().Load(reader);
                ////Statics<M, IdType>.WriteToEmptyProperties(m);
                //m.GetContent().IsNew = false;
                //return m;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on read: {0}\r\n{1}", e.Message, e.StackTrace);
                return default(M);
            }
            finally
            {
                StepReader();
            }
        }

        public M ReadAndClose()
        {
            if (IsDone)
                return default(M);
                //throw new Exception("Reader is closed");

            var m = Read();
            Close();

            return m;
        }


        public IEnumerable<object> GetIds()
        {
            //if (IsDone)
            //    throw new Exception("Reader is closed");

            using (var r = reader)
            {
                while (!IsDone)
                {
                    yield return Peak();
                    StepReader();
                }
            }
        }

        public IEnumerable<M> GetAll()
        {
            //if (IsDone)
            //    throw new Exception("Reader is closed");

            using (var r = reader)
            {
                while (!IsDone)
                {
                    var m = Read();

                    if (m != null)
                        yield return m;
                }
            }
        }

        public void Close()
        {
            if (!IsDone)
            {
                //IsDone = true;
                //reader.Close();
                reader.Dispose();
            }
        }

        private void StepReader()
        {
            if (!IsDone && !reader.IsClosed && !reader.Read())
                Close();
        }

        public void Dispose()
        {
            Close();
        }

        //~Materializer()
        //{
        //    //Dispose();
        //}
    }
}
