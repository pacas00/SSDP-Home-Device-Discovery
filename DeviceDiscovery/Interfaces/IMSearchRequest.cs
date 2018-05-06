using System;
using DeviceDiscovery.Enums;

namespace DeviceDiscovery.Interfaces
{
    public interface IMSearchRequest : IHost, IHeaders
    {
        CastMethod SearchCastMethod { get; }
        string MAN { get; }
        TimeSpan MX { get; }
        IST ST { get; }
        string CPFN { get; }
        string CPUUID { get; }
        string TCPPORT { get; }
    }
}