using DeviceDiscovery.Enums;

namespace DeviceDiscovery.Interfaces
{
    public interface IST
    {
        STtype STtype { get; }
        string DeviceUUID { get; }
        string Type { get; }
        string Version { get; }
        string DomainName { get; }
        bool HasDomain { get; }
    }
}