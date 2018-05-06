using System;
using System.Threading.Tasks;

namespace ConsoleApp1.Interfaces
{
    public interface IControlPoint
    {
        Task<IObservable<IMSearchResponse>> CreateMSearchResponseObservable(int tcpReponsePort);

        Task SendMSearchAsync(IMSearchRequest mSearch);
    }
}