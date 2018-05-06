using System;
using System.Collections.Generic;
using DeviceDiscovery.Enums;
using DeviceDiscovery.Interfaces;
using DeviceDiscovery.Models;
using ISimpleHttpServer.Model;

namespace DeviceDiscovery.Helpers
{
    public static class Convert
    {
        public static int GetMaxAge(IDictionary<string, string> headers)
        {
            var cacheControl = GetHeaderValue(headers, "CACHE-CONTROL");

            if (cacheControl == null) return -0;

            var stringArray = cacheControl.Trim().Split('=');
            var maxAgeStr = stringArray[1];

            var maxAge = 0;
            if (maxAgeStr != null)
            {
                int.TryParse(maxAgeStr, out maxAge);
            }
            return maxAge;
        }

        public static string GetHeaderValue(IDictionary<string, string> headers, string key)
        {
            headers.TryGetValue(key.ToUpper(), out string value);
            return value;
        }

        public static IST GetSTValue(string stString)
        {
            var stringParts = stString.Split(':');

            switch (stringParts[0].ToLower())
            {
                case "ssdp":
                    return new ST
                    {
                        STtype = STtype.All
                    };
                case "uuid":
                    return new ST
                    {
                        STtype = STtype.UIID
                    };
                case "upnp":
                    return new ST
                    {
                        STtype = STtype.RootDevice
                    };
                case "urn":
                    {
                        var st = new ST();
                        if (stringParts[1].ToLower() != "schemas-upnp-org")
                        {
                            st.DomainName = stringParts[1];
                            st.HasDomain = true;
                        }
                        else
                        {
                            st.HasDomain = false;
                        }

                        if (stringParts[2].ToLower() == "device")
                        {
                            st.STtype = STtype.DeviceType;
                        }
                        if (stringParts[2].ToLower() == "service")
                        {
                            st.STtype = STtype.ServiceType;
                        }
                        st.Type = stringParts[3];
                        st.Version = stringParts[4];
                        return st;
                    }
            }

            return null;
        }

        public static Uri UrlToUri(string url)
        {
            Uri.TryCreate(url, UriKind.Absolute, out var uri);
            return uri;

        }

        public static DateTime ToRfc2616Date(string dateString)
        {
            if (dateString != null)
            {
                return DateTime.ParseExact(dateString, "r", null);
            }
            return default(DateTime);
        }

        public static string GetNtsString(NTS nts)
        {
            switch (nts)
            {
                case NTS.Alive: return "ssdp:alive";
                case NTS.ByeBye: return "ssdp:byebye";
                case NTS.Update: return "ssdp:update";

                default:
                    return "<unknown>";
            }
        }

        public static CastMethod GetCastMetod(IHttpCommon request)
        {
            switch (request.RequestType)
            {
                case RequestType.TCP: return CastMethod.Unicast;
                case RequestType.UDP: return CastMethod.Multicast;
                default: return CastMethod.NoCast;
            }
        }

        public static NTS ConvertToNotificationSubTypeEnum(string str)
        {
            switch (str.ToLower())
            {
                case "ssdp:alive": return NTS.Alive;
                case "ssdp:byebye": return NTS.ByeBye;
                case "ssdp:update": return NTS.Update;
                case "ssdp:propchange": return NTS.Propchange;
                default: return NTS.Unknown;
            }
        }
    }
}
