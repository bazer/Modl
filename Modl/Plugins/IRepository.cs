using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Plugins
{
    public interface IRepository
    {
        void WriteCommitsAndModifications(IEnumerable<ICommit> commits);
        void ReadCommits(DateTime since);
        void ReadModifications(IEnumerable<Guid> modifications);

    }
}
