using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Repository
{
    public class CommitContainer : IContainer
    {
        public Dictionary<string, object> Values { get; }
        public About About { get; }
        public string Hash { get; }
        public StorageIdentity Identity { get; }

        public CommitContainer(ICommit commit)
        {
            this.About = GetAbout(commit);
            this.Identity = GetIdentity(commit.Id);
            this.Values = GetValues(commit).ToDictionary(x => x.Key, x => x.Value);
        }

        internal About GetAbout(ICommit commit)
        {
            return new About
            {
                Id = commit.Id,
                Type = "commit",
                Time = DateTime.UtcNow
            };
        }

        internal StorageIdentity GetIdentity(object id)
        {
            return new StorageIdentity
            {
                Id = id,
                IdType = typeof(Guid),
                Name = "commit",
                Type = typeof(ICommit)
            };
        }

        private IEnumerable<KeyValuePair<string, object>> GetValues(ICommit commit)
        {
            yield return new KeyValuePair<string, object>("modifications", commit
                .Modifications
                .Select(x => x.Id)
                .ToList());
        }
    }
}
