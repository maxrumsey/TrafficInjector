using System;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public interface IFetcher
    {
        event EventHandler<Fetcher.FetcherEventArgs<AircraftDTO[]>>? AircraftReceived;

        Task FetchForAllVisCentres();
    }
}