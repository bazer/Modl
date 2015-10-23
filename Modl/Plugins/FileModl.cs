using Modl.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Modl.Structure;
using Modl.Structure.Storage;

namespace Modl.Plugins
{
    public class FileModl : IEndpoint
    {
        string path = "test" + Path.DirectorySeparatorChar;
        string fileEnding = "json";

        public FileModl()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public IEnumerable<Identity> List(Identity identity)
        {
            return Directory
                .EnumerateFiles(GetDirectory(identity))
                .Select(file => new Identity
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Name = identity.Name,
                    Type = identity.Type
                });
        }

        public Stream Get(Identity identity)
        {
            var path = GetPath(identity);

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return File.OpenRead(path);
        }

        public void Save(Identity identity, MemoryStream stream)
        {
            if (!Directory.Exists(GetDirectory(identity)))
                Directory.CreateDirectory(GetDirectory(identity));

            var path = GetPath(identity);

            if (File.Exists(path))
                File.Delete(path);

            using (var fs = File.OpenWrite(path))
            {
                stream.WriteTo(fs);
            }
        }

        public void Delete(Identity identity)
        {
            var path = GetPath(identity);

            if (!File.Exists(path))
                throw new FileNotFoundException();

            File.Delete(path);
        }

        private string GetPath(Identity identity)
        {

            return GetDirectory(identity) + GetFilename(identity);
        }

        private string GetDirectory(Identity identity)
        {

            return path + identity.Name + Path.DirectorySeparatorChar;
        }

        private string GetFilename(Identity identity)
        {
            
            return identity.Id + "." + fileEnding;
        }
    }
}
