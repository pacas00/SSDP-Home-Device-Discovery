using System.Collections.Generic;

namespace ConsoleApp1.Interfaces
{
    public interface IHeaders
    {
        IDictionary<string, string> Headers { get; }
    }
}