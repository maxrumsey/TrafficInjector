using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public class Fetcher
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://opendata.adsb.fi/api/v2/";
        private readonly ILogger _logger;

        public Fetcher(HttpClient httpClient, ILogger<Fetcher> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseUrl);

        }

        public event EventHandler<FetcherEventArgs<AircraftDTO[]>>? AircraftReceived;

        public async Task<HttpResponseMessage> Fetch()
        {
            var res = await _httpClient.GetAsync("lat/-37.68390/lon/144.85104/dist/50");

            return res;
        }

        public async Task<AircraftDTO[]> ParseRequest(HttpResponseMessage content)
        {
            var data = await content.Content.ReadAsStringAsync();

            var parsedResponse = JsonSerializer.Deserialize<ResponseDTO>(data);

            return parsedResponse?.Aircraft ?? Array.Empty<AircraftDTO>();
        }

        public async Task FetchAndFireDTOs()
        {
            var response = await Fetch();

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

        public class FetcherEventArgs<T> : EventArgs
        {
            public T? Data { get; set; }
        }

        private record ResponseDTO
        {
            [JsonPropertyName("aircraft")]
            public AircraftDTO[]? Aircraft { get; set; }
        }
    }
}
