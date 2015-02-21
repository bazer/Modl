using Modl.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Modl.Structure;

namespace Modl.Plugins
{
    public class FileModl : IModlEndpoint
        //<M> : IModlEndpoint<M>
        //where M : IModl
    {
        string path = "test" + Path.DirectorySeparatorChar;
        string fileEnding = "json";

        public FileModl()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public Stream Get(ModlIdentity identity)
        {
            if (!File.Exists(GetPath(identity)))
                throw new FileNotFoundException();

            return File.OpenRead(GetPath(identity));
        }

        public void Save(ModlIdentity identity, MemoryStream stream)
        {
            if (!Directory.Exists(GetDirectory(identity)))
                Directory.CreateDirectory(GetDirectory(identity));

            using (var fs = File.OpenWrite(GetPath(identity)))
            {
                stream.WriteTo(fs);
            }
        }

        private string GetPath(ModlIdentity identity)
        {

            return GetDirectory(identity) + GetFilename(identity);
        }

        private string GetDirectory(ModlIdentity identity)
        {

            return path + identity.ModlName + Path.DirectorySeparatorChar;
        }

        private string GetFilename(ModlIdentity identity)
        {
            
            return identity.Id + "." + fileEnding;
        }
    }
}
