using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public interface ICommit
    {
        Guid Id { get; }
        DateTime When { get; }
        IUser Who { get; }
        IEnumerable<ICommit> Previous { get; }
        IEnumerable<ICommit> Next { get; }
    }

    public class Commit : ICommit
    {
        public Guid Id => throw new NotImplementedException();

        public DateTime When => throw new NotImplementedException();

        public IUser Who => throw new NotImplementedException();

        public IEnumerable<ICommit> Previous => throw new NotImplementedException();

        public IEnumerable<ICommit> Next => throw new NotImplementedException();
    }
}
