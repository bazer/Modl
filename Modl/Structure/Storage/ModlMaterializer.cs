using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Structure.Storage
{
    public class ModlMaterializer
    {
        internal static IEnumerable<ModlStorage> Get(IEnumerable<ModlIdentity> identities, ModlSettings settings)
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

        internal static void Save(IEnumerable<ModlStorage> storages, ModlSettings settings)
        {
            foreach (var storage in storages)
            {
                var stream = settings.Serializer.Serialize(storage);
                stream.Position = 0;

                settings.Endpoint.Save(storage.Identity, stream);

                stream.Dispose();
            }
        }
    }
}
