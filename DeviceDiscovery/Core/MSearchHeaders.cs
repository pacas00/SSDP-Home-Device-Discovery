using System.Collections.Generic;

namespace DeviceDiscovery.Core
{
    internal static class MSearchHeaders
    {
        internal static readonly List<string> AllHeaders = new List<string>
        {
            CacheControl, Location, Date, Ext, Server, SearchTarget, Usn
        };
        internal const string CacheControl = "Cache-Control";
        internal const string Location = "Location";
        internal const string Date = "Date";
        internal const string Ext = "Ext";
        internal const string Server = "Server";
        internal const string SearchTarget = "St";
        internal const string Usn = "Usn";
    }
}