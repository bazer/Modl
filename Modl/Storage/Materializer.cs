using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Storage
{
    public class Materializer
    {
        internal static IEnumerable<object> List(Identity identity, Settings settings)
        {
            return settings.Endpoint
                .List(identity)
                .Select(x => DeserializeObject(x.Id, identity.IdType, settings));
        }

        internal static IEnumerable<Container> Read(IEnumerable<Identity> identities, Settings settings)
        {
            foreach (var identity in identities)
            {
                var stream = settings.Endpoint.Get(identity);
                stream.Position = 0;
                var storage = settings.Serializer.Deserialize(stream);
                stream.Dispose();

                yield return storage;
            }
        }

        internal static void Write(IEnumerable<Container> storages, Settings settings)
        {
            foreach (var storage in storages)
            {
                var stream = settings.Serializer.Serialize(storage);
                stream.Position = 0;

                settings.Endpoint.Save(storage.Identity, stream);

                stream.Dispose();
            }
        }

        internal static void Delete(IEnumerable<Container> storages, Settings settings)
        {
            foreach (var storage in storages)
            {
                settings.Endpoint.Delete(storage.Identity);
            }
        }

        internal static object DeserializeObject(object obj, Type toType, Settings settings)
        {
            return settings.Serializer.DeserializeObject(obj, toType);
        }
    }
}
