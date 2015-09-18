using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Interfaces;
using Newtonsoft.Json;
using System.IO;
using Modl.Structure.Storage;

namespace Modl.Json
{
    public class JsonModl: ISerializer
    {
        MemoryStream ISerializer.Serialize(Storage storage)
        {
            var stream = new MemoryStream();
            var jsonTextWriter = new JsonTextWriter(new StreamWriter(stream));
            jsonTextWriter.Formatting = Formatting.Indented;

            new JsonSerializer().Serialize(jsonTextWriter, storage);
            //new JsonSerializer().Serialize(jsonTextWriter, storage.Values);
            jsonTextWriter.Flush();

            return stream;
        }

        Storage ISerializer.Deserialize(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<Storage>(jsonTextReader);
                //return new ModlStorage(serializer.Deserialize<Dictionary<string, object>>(jsonTextReader));
            }
        }

        object ISerializer.DeserializeObject(object obj, Type toType)
        {
            return JsonConvert.DeserializeObject(obj.ToString(), toType);
        }
    }
}
