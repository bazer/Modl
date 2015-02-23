using System;

namespace Modl.Structure
{
    public class ModlIdentity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Version { get; set; }
        public DateTime Timestamp { get; set; }
        public object Modifier { get; set; }
        public string Valuehash { get; set; }
        public string Identityhash { get; set; }
    }
}
