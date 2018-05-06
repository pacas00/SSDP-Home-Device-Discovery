using DeviceDiscovery.Enums;
using DeviceDiscovery.Interfaces;

namespace DeviceDiscovery.Models
{
    internal class ST : IST
    {
        public STtype STtype { get; internal set; }
        public string DeviceUUID { get; internal set; }
        public string Type { get; internal set; }
        public string Version { get; internal set; }
        public string DomainName { get; internal set; }
        public bool HasDomain { get; internal set; }
    }
}
