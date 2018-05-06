using ConsoleApp1.Enums;

namespace ConsoleApp1.Interfaces
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