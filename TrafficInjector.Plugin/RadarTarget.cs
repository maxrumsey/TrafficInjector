using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class RadarTarget : RDP.RadarTrack
    {
        private int _lastAltitude = -1;

        public string Callsign { get; set; }

        public string Hex { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public Coordinate Location { get; set; } = null!;

        public Track? Track { get; set; }

        public RadarTarget(AircraftDTO dto)
        {
            ActualAircraft = new()
            {
                Callsign = dto.Callsign!,
                TransponderModeC = true,
            };

            Callsign = dto.Callsign!;
            Hex = dto.HexCode!;

            RadarTypes = RDP.RadarTypes.ADSB | RDP.RadarTypes.SSR_ModeS | RDP.RadarTypes.PRI;

            Update(dto);
        }

        public void Update(AircraftDTO dto)
        {

            Location = new(dto.Latitude.GetValueOrDefault(), dto.Longitude.GetValueOrDefault());

            ActualAircraft.PressureAltitude = dto.Altitude!.Value;
            ActualAircraft.TrueAltitude = dto.Altitude!.Value;
            ActualAircraft.GroundSpeed = (int)dto.GroundSpeed;
            ActualAircraft.Heading = (int)dto.Track;
            ActualAircraft.Position = Location;

            AboveTransition = true;
            LatLong = Location;
            GroundSpeed = dto.GroundSpeed;
            Heading = dto.Track;
            CorrectedAltitude = dto.Altitude!.Value;

            if (_lastAltitude != -1)
            {
                var deltaTime = (DateTime.Now - LastUpdated).TotalMinutes;
                VerticalSpeed = (CorrectedAltitude - _lastAltitude) / deltaTime;
            }

            LastUpdated = DateTime.Now;

            Timestamp = LastUpdated;

            _lastAltitude = CorrectedAltitude;

            AddPositionHistory(new(Location, GroundSpeed, Heading, LastUpdated, false));

            if (dto.SelectedAltitude is not null && QuickTag is not null)
            {
                QuickTag.CFL = dto.SelectedAltitude.Value;
            }
        }
    }
}
