using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeviceDiscovery.Core;
using DeviceDiscovery.Models;

namespace DeviceDiscovery
{
    public class DiscoveryService
    {
        public List<MSearchResponse> LocateDevices(MSearchRequest mSearchRequest)
        {
            var devices = new List<MSearchResponse>();

            if (mSearchRequest.Timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException("Timeout value must be greater than zero.", nameof(mSearchRequest.Timeout));
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("M-SEARCH * HTTP/1.1\r\n");
            stringBuilder.Append($"HOST: {Constants.MulticastAdress}:{mSearchRequest.MulsticastPort}\r\n");
            stringBuilder.Append("MAN: \"ssdp:discover\"\r\n");
            stringBuilder.Append("MX: 1\r\n");
            stringBuilder.Append($"ST: {SelectSearchTarget(mSearchRequest.ST)}\r\n\r\n");

            var request = Encoding.UTF8.GetBytes(stringBuilder.ToString());

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, mSearchRequest.UnicastPort));
                socket.SetSocketOption(
                    SocketOptionLevel.IP,
                    SocketOptionName.AddMembership,
                    new MulticastOption(mSearchRequest.MulticastAddress, IPAddress.Any)
                );

                var thd = new Task(() => GetSocketResponse(socket, mSearchRequest.MulsticastPort, devices));

                socket.SendTo(
                    request,
                    0,
                    request.Length,
                    SocketFlags.None,
                    new IPEndPoint(mSearchRequest.MulticastAddress, mSearchRequest.MulsticastPort)
                );

                thd.Start();
                Thread.Sleep(mSearchRequest.Timeout);
            }

            return devices.GroupBy(x => x.Location).Select(group => group.First()).ToList();
        }

        private void GetSocketResponse(Socket socket, int port, List<MSearchResponse> devices)
        {
            try
            {
                while (true)
                {
                    var response = new byte[512];
                    EndPoint ep = new IPEndPoint(IPAddress.Any, port);
                    socket.ReceiveFrom(response, ref ep);

                    try
                    {
                        var cleanResponse = RemoveEmptyBytesFromResponse(response);
                        var receivedString = Encoding.UTF8.GetString(cleanResponse);

                        var mSearchResponse = new MSearchResponse(receivedString);

                        devices.Add(mSearchResponse);
                    }
                    catch(Exception e)
                    {

                    }
                }
            }
            catch
            {
                //TODO handle exception for when connection closes
            }
        }

        private string SelectSearchTarget(SearchTarget searchTarget)
        {
            switch (searchTarget)
            {
                case SearchTarget.All:
                    return "SsdpSearch:all";
                case SearchTarget.Nanoleaf:
                    return "nanoleaf_aurora:light";
                case SearchTarget.Yeelight:
                    return "wifi_bulb";
                default:
                    return null;
            }
        }

        private static byte[] RemoveEmptyBytesFromResponse(byte[] response)
        {
            int i = response.Length - 1;
            while (response[i] == 0)
            {
                --i;
            }
            var bar = new byte[i + 1];
            Array.Copy(response, bar, i + 1);

            return bar;
        }
    }
}