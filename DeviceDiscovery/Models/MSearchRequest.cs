using System;
using System.Net;
using DeviceDiscovery.Core;

namespace DeviceDiscovery.Models
{
    public class MSearchRequest
    {
        internal IPAddress MulticastAddress => IPAddress.Parse(Constants.MulticastAdress);

        public int MulsticastPort { get; set; } = 1900;

        public int UnicastPort { get; set; }

        public SearchTarget ST { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}