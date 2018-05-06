using System;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDiscovery.Enums;
using DeviceDiscovery.Helpers;
using DeviceDiscovery.Interfaces;
using DeviceDiscovery.Models;
using ISimpleHttpServer.Service;

namespace DeviceDiscovery
{
    public class ControlPoint : IControlPoint
    {
        private readonly IHttpListener _httpListener;

        public async Task<IObservable<IMSearchResponse>> CreateMSearchResponseObservable(int tcpReponsePort)
        {
            var multicastResObs = await _httpListener.UdpMulticastHttpResponseObservable("239.255.255.250", 1982);

            var tcpResObs = await _httpListener.TcpHttpResponseObservable(tcpReponsePort);

            return multicastResObs
                .Merge(tcpResObs)
                .Where(x => !x.IsUnableToParseHttp && !x.IsRequestTimedOut)
                .Select(res => new MSearchResponse(res));
        }

        public ControlPoint(IHttpListener httpListener)
        {
            _httpListener = httpListener;
        }

        public async Task SendMSearchAsync(IMSearchRequest mSearch)
        {
            if (mSearch.SearchCastMethod == CastMethod.Multicast)
            {
                await _httpListener.SendOnMulticast(ComposeMSearchDatagram(mSearch));
            }
        }

        private byte[] ComposeMSearchDatagram(IMSearchRequest request)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("M-SEARCH * HTTP/1.1\r\n");

            stringBuilder.Append(request.SearchCastMethod == CastMethod.Multicast
                ? "HOST: 239.255.255.250:1982\r\n"
                : $"HOST: {request.Name}:{request.Port}\r\n");

            stringBuilder.Append("MAN: \"ssdp:discover\"\r\n");

            if (request.SearchCastMethod == CastMethod.Multicast)
            {
                stringBuilder.Append($"MX: {request.MX.TotalSeconds}\r\n");
            }
            stringBuilder.Append($"ST: {GetSTSting(request.ST)}\r\n");

            if (request.SearchCastMethod == CastMethod.Multicast)
            {
                stringBuilder.Append($"CPFN.UPNP.ORG: {request.CPFN}\r\n");

                HeaderHelper.AddOptionalHeader(stringBuilder, "TCPPORT.UPNP.ORG", request.TCPPORT);
                HeaderHelper.AddOptionalHeader(stringBuilder, "CPUUID.UPNP.ORG", request.CPUUID);

                if (request.Headers != null)
                {
                    foreach (var header in request.Headers)
                    {
                        stringBuilder.Append($"{header.Key}: {header.Value}\r\n");
                    }
                }
            }

            stringBuilder.Append("\r\n");
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        private string GetSTSting(IST st)
        {
            switch (st.STtype)
            {
                case STtype.All: return "ssdp:all";
                //case STtype.Nanoleaf: return "nanoleaf_aurora:light";
                case STtype.Nanoleaf: return "wifi_bulb";
                case STtype.RootDevice: return "upnp:rootdevice";
                case STtype.UIID:
                    {
                        return $"uuid:{st.DeviceUUID}";
                    }
                case STtype.DeviceType:
                {
                    return st.HasDomain 
                        ? $"urn:{st.DomainName}:device:{st.Type}:{st.Version}"
                        : $"urn:schemas-upnp-org:device:{st.Type}:{st.Version}";
                }
                case STtype.ServiceType:
                {
                    return st.HasDomain 
                        ? $"urn:{st.DomainName}:service:{st.Type}:{st.Version}"
                        : $"urn:schemas-upnp-org:service:{st.Type}:{st.Version}";
                }
            }

            return null;
        }
    }
}
