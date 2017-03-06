using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Repository
{
    internal class Commits
    {
        private static Commits StaticInstance;

        [ThreadStatic]
        private static Commits ThreadInstance;

        private ConcurrentDictionary<Guid, ICommit> commits = new ConcurrentDictionary<Guid, ICommit>();

        public static Commits ForThisModl
        {
            get
            {
                if (Settings.GlobalSettings.InstanceSeparation == InstanceSeparation.None)
                {
                    if (StaticInstance == null)
                        StaticInstance = new Commits();

                    return StaticInstance;
                }
                else if (Settings.GlobalSettings.InstanceSeparation == InstanceSeparation.Thread)
                {
                    if (ThreadInstance == null)
                        ThreadInstance = new Commits();

                    return ThreadInstance;
                }
                else
                    throw new NotImplementedException();
                //else
                //{
                //    return Settings.GlobalSettings.CustomInstanceSeparationDictionary()
                //        .GetOrAdd(typeof(M), type => new CommitStore()) as CommitStore;
                //}
            }
        }
        
        public void AddCommit(ICommit commit)
        {
            commits.TryAdd(commit.Id, commit);
        }

        public ICommit GetCommit(Guid id)
        {
            if (!commits.TryGetValue(id, out ICommit commit))
                throw new KeyNotFoundException($"Commit with id '{id}' not found");

            return commit;
        }


    }
}
