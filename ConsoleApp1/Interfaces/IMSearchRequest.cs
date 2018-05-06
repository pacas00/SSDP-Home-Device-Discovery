using System;
using ConsoleApp1.Enums;

namespace ConsoleApp1.Interfaces
{
    public interface IMSearchRequest : IHost, IHeaders
    {
        CastMethod SearchCastMethod { get; }
        string MAN { get; }
        TimeSpan MX { get; }
        IST ST { get; }
        string CPFN { get; }
        string CPUUID { get; }
        string TCPPORT { get; }
    }
}