using System;
using System.Collections.Generic;
using System.Linq;
using DeviceDiscovery.Core;

namespace DeviceDiscovery.Helpers
{
    internal static class HeaderHelpers
    {
        internal static int GetMaxAge(IDictionary<string, string> headers)
        {
            var cacheControl = GetHeaderValue(headers, MSearchHeaders.CacheControl);

            if (cacheControl == null)
            {
                return -0;
            }

            var stringArray = cacheControl.Trim().Split('=');
            var maxAgeStr = stringArray[1];

            var maxAge = 0;
            if (maxAgeStr != null)
            {
                int.TryParse(maxAgeStr, out maxAge);
            }
            return maxAge;
        }

        internal static Uri GetUri(IDictionary<string, string> headers)
        {
            var headerValue = GetHeaderValue(headers, MSearchHeaders.Location);
            Uri.TryCreate(headerValue, UriKind.Absolute, out var uri);

            return uri;
        }

        internal static DateTime GetDate(IDictionary<string, string> headers)
        {
            var headerValue = GetHeaderValue(headers, MSearchHeaders.Date);
            if (!string.IsNullOrEmpty(headerValue) && !string.IsNullOrWhiteSpace(headerValue))
            {
                return DateTime.Parse(headerValue);
            }

            return default(DateTime);
        }

        internal static IDictionary<string, string> GetNotStandardHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in MSearchHeaders.AllHeaders)
            {
                headers.Remove(header);
            }

            return headers;
        }

        internal static string GetHeaderValue(IDictionary<string, string> headers, string key)
        {
            return headers.FirstOrDefault(x => x.Key.ToUpperInvariant().Equals(key.ToUpperInvariant())).Value;
        }
    }
}