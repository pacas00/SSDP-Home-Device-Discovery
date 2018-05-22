using System;
using System.Collections.Generic;
using System.Linq;
using DeviceDiscovery.Core;
using DeviceDiscovery.Helpers;

namespace DeviceDiscovery.Models
{
    public class MSearchResponse
    {
        public TimeSpan CacheControl { get; }

        public DateTime Date { get; }

        public bool Ext { get; }

        public Uri Location { get; }

        public string ServerInfo { get; }

        public string SearchTarget { get; }

        public string Usn { get; }

        public IDictionary<string, string> Headers { get; }

        internal MSearchResponse(string mSearchResponse)
        {
            var responseKeyValues = mSearchResponse.Split("\r\n");

            Dictionary<string, string> headers = responseKeyValues.Select(responseKeyValue => responseKeyValue.Split(':', 2)).Where(test1 => test1.Length == 2).ToDictionary(test1 => test1[0], test1 => test1[1]);
            try
            {
                CacheControl = TimeSpan.FromSeconds(HeaderHelpers.GetMaxAge(headers));
                Location = HeaderHelpers.GetUri(headers);
                Date = HeaderHelpers.GetDate(headers);
                Ext = headers.ContainsKey(MSearchHeaders.Ext);
                ServerInfo = HeaderHelpers.GetHeaderValue(headers, MSearchHeaders.Server);
                SearchTarget = HeaderHelpers.GetHeaderValue(headers, MSearchHeaders.SearchTarget);
                Usn = HeaderHelpers.GetHeaderValue(headers, MSearchHeaders.Usn);
                Headers = HeaderHelpers.GetNotStandardHeaders(headers);
            }
            catch (Exception)
            {

            }
        }
    }
}