using Modl.Structure;
using Modl.Structure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Storage;

namespace Modl.Repository
{
    public class ChangeContainer : IContainer
    {
        public Dictionary<string, object> Values { get; }
        public About About { get; }
        public string Hash { get; }
        public StorageIdentity Identity { get; }
        public Guid Property { get; }

        public ChangeContainer(IChange change)
        {
            this.About = GetAbout(change);
            this.Identity = GetIdentity(change.Id);
            //this.Property = change.Property.Id;
            this.Values = GetValues(change).ToDictionary(x => x.Key, x => x.Value);
        }

        internal About GetAbout(IChange change)
        {
            return new About
            {
                Id = change.Id,
                Type = "change",
                Time = DateTime.UtcNow
            };
        }

        //internal Values GetValues(IChange change)
        //{
        //    return new Values
        //    {
        //        Old = change.OldValue.Content.ToString(),
        //        New = change.NewValue.Content.ToString(),
        //    };
        //}

        internal StorageIdentity GetIdentity(object id)
        {
            return new StorageIdentity
            {
                Id = id,
                IdType = typeof(Guid),
                Name = "change",
                Type = typeof(IChange)
            };
        }

        private IEnumerable<KeyValuePair<string, object>> GetValues(IChange change)
        {
            if (change.OldValue != null)
                yield return new KeyValuePair<string, object>("Old", change.OldValue.Content);

            if (change.NewValue != null)
                yield return new KeyValuePair<string, object>("New", change.NewValue.Content);


            //if (change.Property.Metadata.IsLink)
            //{
            //    yield return new KeyValuePair<string, object>("OldProperty", new { name = change.OldProperty.Name, value = (change.OldProperty as IRelationProperty)?.RelationId.Get() });
            //    yield return new KeyValuePair<string, object>("NewProperty", new { name = change.NewProperty.Name, value = (change.NewProperty as IRelationProperty)?.RelationId.Get() });
            //}
            //else
            //{
            //    yield return new KeyValuePair<string, object>("OldProperty", new { name = change.OldProperty.Name, value = (change.OldProperty as ISimpleProperty)?.Value });
            //    yield return new KeyValuePair<string, object>("NewProperty", new { name = change.NewProperty.Name, value = (change.NewProperty as ISimpleProperty)?.Value });
            //}
        }
    }
}
