using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Repository
{
    public class MutationContainer : IContainer
    {
        public Dictionary<string, object> Values { get; }
        public About About { get; }
        public string Hash { get; }
        public StorageIdentity Identity { get; }

        public MutationContainer(IMutation mutation)
        {
            this.About = GetAbout(mutation);
            this.Identity = GetIdentity(mutation.Id);
            this.Values = GetValues(mutation).ToDictionary(x => x.Key, x => x.Value);
        }

        internal About GetAbout(IMutation mutation)
        {
            return new About
            {
                Id = mutation.Id,
                Type = "mutation",
                Time = DateTime.UtcNow
            };
        }

        internal StorageIdentity GetIdentity(object id)
        {
            return new StorageIdentity
            {
                Id = id,
                IdType = typeof(Guid),
                Name = "mutation",
                Type = typeof(IMutation)
            };
        }

        private IEnumerable<KeyValuePair<string, object>> GetValues(IMutation mutation)
        {
            if (mutation.OldProperty.Metadata.IsLink)
            {
                yield return new KeyValuePair<string, object>("OldProperty", new { name = mutation.OldProperty.Name, value = (mutation.OldProperty as IRelationProperty)?.Value });
                yield return new KeyValuePair<string, object>("NewProperty", new { name = mutation.NewProperty.Name, value = (mutation.NewProperty as IRelationProperty)?.Value });
            }
            else
            {
                yield return new KeyValuePair<string, object>("OldProperty", new { name = mutation.OldProperty.Name, value = (mutation.OldProperty as ISimpleProperty)?.Value });
                yield return new KeyValuePair<string, object>("NewProperty", new { name = mutation.NewProperty.Name, value = (mutation.NewProperty as ISimpleProperty)?.Value });
            }
        }
    }
}
