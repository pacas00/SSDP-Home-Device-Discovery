using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp1.Enums;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Models;
using ISimpleHttpServer.Service;
using ISocketLite.PCL.Interface;
using SimpleHttpServer.Service;
using SocketLite.Model;

namespace ConsoleApp1
{
    class Program
    {
        private static IHttpListener _httpListener;
        private static IControlPoint _controlPoint;
        private static string _controlPointLocalIp = "192.168.0.106";

        static void Main()
        {
            Console.WriteLine("Searching local network for Nanoleaf device...");

            StartAsync();

            Console.ReadKey();
        }

        private static async void StartAsync()
        {
            _httpListener = await GetHttpListener(_controlPointLocalIp);

            await StartControlPointListeningAsync();
        }

        private static async Task StartControlPointListeningAsync()
        {
            _controlPoint = new ControlPoint(_httpListener);

            await ListenToMSearchResponse();
        }

        private static async Task ListenToMSearchResponse()
        {
            var mSeachResObs = await _controlPoint.CreateMSearchResponseObservable(1901);

            var counter = 0;

            mSeachResObs
                .Subscribe(
                    res =>
                    {
                        counter++;
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"---### Control Point Received a  M-SEARCH RESPONSE #{counter} ###---");
                        Console.ResetColor();
                        Console.WriteLine($"{res.ResponseCastMethod.ToString()}");
                        Console.WriteLine($"From: {res.Name}:{res.Port}");
                        Console.WriteLine($"Status code: {res.StatusCode} {res.ResponseReason}");
                        Console.WriteLine($"Location: {res.Location.AbsoluteUri}");
                        Console.WriteLine($"Date: {res.Date.ToString(CultureInfo.CurrentCulture)}");
                        Console.WriteLine($"Cache-Control: max-age = {res.CacheControl}");
                        Console.WriteLine($"ST: {res.ST}");
                        Console.WriteLine($"USN: {res.USN}");
                        Console.WriteLine($"BOOTID.UPNP.ORG: {res.BOOTID}");
                        Console.WriteLine($"CONFIGID.UPNP.ORG: {res.CONFIGID}");
                        Console.WriteLine($"SEARCHPORT.UPNP.ORG: {res.SEARCHPORT}");
                        Console.WriteLine($"SECURELOCATION: {res.SECURELOCATION}");

                        if (res.Headers != null)
                        {
                            if (res.Headers.Any())
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine($"Additional Headers: {res.Headers.Count}");
                                foreach (var header in res.Headers)
                                {
                                    Console.WriteLine($"{header.Key}: {header.Value}; ");
                                }

                                Console.ResetColor();
                            }
                        }

                        Console.WriteLine();
                    });

            await StartMSearchRequestMulticastAsync();
        }

        private static async Task StartMSearchRequestMulticastAsync()
        {
            var mSearchMessage = new MSearch
            {
                SearchCastMethod = CastMethod.Multicast,
                CPFN = "TestXamarin",
                Name = "239.255.255.250",
                Port = 1982,
                MX = TimeSpan.FromSeconds(5),
                TCPPORT = "1901",
                ST = new ST
                {
                    STtype = STtype.Nanoleaf
                }
            };
            await _controlPoint.SendMSearchAsync(mSearchMessage);
        }

        internal class MSearch : IMSearchRequest
        {
            public string Name { get; internal set; }
            public int Port { get; internal set; }
            public IDictionary<string, string> Headers { get; internal set; }
            public CastMethod SearchCastMethod { get; internal set; }
            public string MAN { get; internal set; }
            public TimeSpan MX { get; internal set; }
            public IST ST { get; internal set; }
            public string CPFN { get; internal set; }
            public string CPUUID { get; internal set; }
            public string TCPPORT { get; internal set; }
        }





        public static async Task<IHttpListener> GetHttpListener(
            string ipAddress,
            TimeSpan timeout = default(TimeSpan))
        {
            var communicationInterface = new CommunicationsInterface();
            var allInterfaces = communicationInterface.GetAllInterfaces();

            var firstUsableInterface = allInterfaces.FirstOrDefault(x => x.IpAddress == ipAddress);

            if (firstUsableInterface == null) throw new ArgumentException($"Unable to locate any network communication interface with the ip address: {ipAddress}");

            return await GetHttpListener(firstUsableInterface, timeout);
        }

        public static async Task<IHttpListener> GetHttpListener(
            ICommunicationInterface communicationInterface,
            TimeSpan timeout = default(TimeSpan))
        {
            if (timeout == default(TimeSpan))
            {
                timeout = TimeSpan.FromSeconds(30);
            }

            return new HttpListener(communicationInterface, timeout);
        }
    }
}