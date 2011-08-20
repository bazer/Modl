using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Modl.Fields;
using System.Threading.Tasks;

namespace Modl.DataAccess
{
    public class Materializer<M, IdType> : IDisposable
        where M : Modl<M, IdType>, new()
    {
        private DbDataReader reader;
        private Database database;

        public bool IsDone { get { return reader.IsClosed; } }
        //public bool IsDone { get; private set; }

        private IdType id;
        private bool isIdSet = false;

        public static Task<Materializer<M, IdType>> Async(Task<DbDataReader> reader, Database database)
        {
            return reader.ContinueWith(r =>
            {
                return new Materializer<M, IdType>(r.Result, database);
            });
        }

        public Materializer(DbDataReader reader, Database database)
        {
            this.reader = reader;
            this.database = database;

            StepReader();
        }


        public IdType Peak()
        {
            if (IsDone)
                throw new Exception("Reader is closed");

            if (!isIdSet)
            {
                id = Helper.GetSafeValue<IdType>(reader, Statics<M, IdType>.IdName);
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
                var m = Modl<M, IdType>.New(database);
                m.Store.Load(reader);
                Statics<M, IdType>.WriteToEmptyProperties(m);
                m.isNew = false;
                return m;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on read: {0}\r\n{1}", e.Message, e.StackTrace);
                return null;
            }
            finally
            {
                StepReader();
            }
        }

        public M ReadAndClose()
        {
            if (IsDone)
                throw new Exception("Reader is closed");

            var m = Read();
            Close();

            return m;
        }


        public IEnumerable<IdType> GetIds()
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
