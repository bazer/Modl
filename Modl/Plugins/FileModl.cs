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
        string savePath;
        string fileEnding = "json";

        public bool CanGenerateIds => false;

        public FileModl(string path)
        {
            this.savePath = path + Path.DirectorySeparatorChar;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public IEnumerable<StorageIdentity> List(StorageIdentity identity)
        {
            return Directory
                .EnumerateFiles(GetDirectory(identity))
                .Select(file => new StorageIdentity
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Name = identity.Name,
                    Type = identity.Type
                });
        }

        public Stream Get(StorageIdentity identity)
        {
            var path = GetPath(identity);

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return File.OpenRead(path);
        }

        public void Save(StorageIdentity identity, MemoryStream stream)
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

        public void Delete(StorageIdentity identity)
        {
            var path = GetPath(identity);

            if (!File.Exists(path))
                throw new FileNotFoundException();

            File.Delete(path);
        }

        private string GetPath(StorageIdentity identity)
        {

            return GetDirectory(identity) + GetFilename(identity);
        }

        private string GetDirectory(StorageIdentity identity)
        {

            return savePath + identity.Name + Path.DirectorySeparatorChar;
        }

        private string GetFilename(StorageIdentity identity)
        {
            
            return identity.Id + "." + fileEnding;
        }
    }
}
