using System;
using ConsoleApp1.Enums;

namespace ConsoleApp1.Interfaces
{
    public interface IMSearchResponse : IHost, IHeaders
    {
        CastMethod ResponseCastMethod { get; }
        int StatusCode { get; }
        string ResponseReason { get; }
        TimeSpan CacheControl { get; }
        DateTime Date { get; }
        Uri Location { get; }
        bool Ext { get; }
        IST ST { get; }
        string USN { get; }
        string BOOTID { get; }
        string CONFIGID { get; }
        string SEARCHPORT { get; }
        string SECURELOCATION { get; }
    }
}