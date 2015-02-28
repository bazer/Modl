using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Interfaces;
using Newtonsoft.Json;
using System.IO;

namespace Modl.Json
{
    public class JsonModl: IModlSerializer
    {
        MemoryStream IModlSerializer.Serialize(ModlStorage storage)
        {
            var stream = new MemoryStream();
            var jsonTextWriter = new JsonTextWriter(new StreamWriter(stream));
            jsonTextWriter.Formatting = Formatting.Indented;

            new JsonSerializer().Serialize(jsonTextWriter, storage);
            //new JsonSerializer().Serialize(jsonTextWriter, storage.Values);
            jsonTextWriter.Flush();

            return stream;
        }

        ModlStorage IModlSerializer.Deserialize(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<ModlStorage>(jsonTextReader);
                //return new ModlStorage(serializer.Deserialize<Dictionary<string, object>>(jsonTextReader));
            }
        }
    }
}
