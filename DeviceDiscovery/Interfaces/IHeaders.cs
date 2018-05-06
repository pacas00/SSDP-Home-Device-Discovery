using System.Collections.Generic;

namespace DeviceDiscovery.Interfaces
{
    public interface IHeaders
    {
        IDictionary<string, string> Headers { get; }
    }
}