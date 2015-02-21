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
        //<M> : IModlSerializer<M>
        //where M : IModl
    {
        public M ConvertFrom<M>(Stream stream) where M : IModl, new()
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<M>(jsonTextReader);
            }
        }

        public MemoryStream ConvertTo<M>(M modl) where M : IModl
        {
            var serializer = new JsonSerializer();

            var stream = new MemoryStream();

            var sr = new StreamWriter(stream);
            var jsonTextWriter = new JsonTextWriter(sr);

            serializer.Serialize(jsonTextWriter, modl);
            jsonTextWriter.Flush();

            return stream;
        }
    }
}
