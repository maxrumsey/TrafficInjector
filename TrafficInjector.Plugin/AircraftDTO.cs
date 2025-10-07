using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public record AircraftDTO
    {
        [JsonPropertyName("hex")]
        public string? HexCode { get; set; }

        [JsonPropertyName("flight")]
        public string? Callsign { get; set; }

        [JsonPropertyName("alt_geom")]
        public int? Altitude { get; set; }

        [JsonPropertyName("lat")]
        public float? Latitude { get; set; }

        [JsonPropertyName("lon")]
        public float? Longitude { get; set; }

        [JsonPropertyName("track")]
        public float Track { get; set; } = 0;

        [JsonPropertyName("gs")]
        public float GroundSpeed { get; set; } = 0;

        [JsonPropertyName("nav_altitude_mcp")]
        public int? SelectedAltitude { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(HexCode) &&
                   !string.IsNullOrWhiteSpace(Callsign) &&
                   Altitude.HasValue &&
                   Latitude.HasValue &&
                   Longitude.HasValue;
        }
    }
}
