using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class Fetcher : IFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://opendata.adsb.fi/api/v2/";
        private readonly ILogger _logger;
        private readonly IvatSysAccessor _vatSys;

        public Fetcher(HttpClient httpClient, ILogger<Fetcher> logger, IvatSysAccessor vatSys)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _vatSys = vatSys;

        }

        public event EventHandler<FetcherEventArgs<AircraftDTO[]>>? AircraftReceived;

        public async Task<HttpResponseMessage> Fetch(Coordinate coord)
        {
            var res = await _httpClient.GetAsync($"lat/{coord.Latitude}/lon/{coord.Longitude}/dist/250");

            return res;
        }

        public static async Task<AircraftDTO[]> ParseRequest(HttpResponseMessage content)
        {
            var data = await content.Content.ReadAsStringAsync();

            var parsedResponse = JsonSerializer.Deserialize<ResponseDTO>(data);

            return parsedResponse?.Aircraft ?? [];
        }

        public async Task FetchAndFireDTOs(Coordinate coord)
        {
            var response = await Fetch(coord);

            if (response.IsSuccessStatusCode)
            {
                var dtos = await ParseRequest(response);

                var validDtos = dtos.Where(d => d.IsValid()).ToArray();

                if (validDtos.Length > 0)
                {
                    AircraftReceived?.Invoke(this, new FetcherEventArgs<AircraftDTO[]>() { Data = validDtos });
                }
            }
        }

        public async Task FetchForAllVisCentres()
        {
            var visCentres = _vatSys.GetVisCentres() ?? throw new Exception("Could not fetch vis centres.");

            foreach (var ctr in visCentres)
            {
                try
                {

                    await FetchAndFireDTOs(ctr);

                }
                catch (Exception ex)
                {
                    Errors.Add(ex, "Traffic Injector");
                }
                finally
                {

                    await Task.Delay(1000);
                }
            }
        }

        public class FetcherEventArgs<T> : EventArgs
        {
            public T? Data { get; set; }
        }

        internal record ResponseDTO
        {
            [JsonPropertyName("aircraft")]
            public AircraftDTO[]? Aircraft { get; set; }
        }
    }
}
