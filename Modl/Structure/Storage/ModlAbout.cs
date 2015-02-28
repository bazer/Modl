using System;

namespace Modl.Structure
{
    public class ModlAbout
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Version { get; set; }
        public DateTime Time { get; set; }
        public object ModifiedBy { get; set; }
        //public string Valuehash { get; set; }
        //public string Identityhash { get; set; }
    }
}
