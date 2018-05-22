using System;
using DeviceDiscovery.Models;

namespace DeviceDiscovery.Test
{
    public class Program
    {
        private static readonly DiscoveryService Ds = new DiscoveryService();

        public static void Main()
        {
            var request = new MSearchRequest
            {
                MulsticastPort = 1982,
                ST = SearchTarget.Yeelight,
                Timeout = TimeSpan.FromSeconds(3),
                UnicastPort = 1901
            };

            var services = Ds.LocateDevices(request);

            foreach (var service in services)
            {
                Console.WriteLine(service.Location);
            }

            Console.ReadKey();
        }
    }
}