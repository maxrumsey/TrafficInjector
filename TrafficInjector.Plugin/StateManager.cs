using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class StateManager
    {
        public StateManager(Fetcher fetcher)
        {
            fetcher.AircraftReceived += AircraftDataReceived; ;
        }

        private void AircraftDataReceived(object sender, Fetcher.FetcherEventArgs<AircraftDTO[]> e)
        {
            if (e.Data is null)
            {
                return;
            }

            foreach (var dto in e.Data)
            {
                if (dto is null || !dto.IsValid())
                {
                    continue;
                }

                var networkPilot = new NetworkPilot()
                {
                    Callsign = dto.Callsign!,
                    PressureAltitude = dto.Altitude!.Value,
                };

                var rt = new vatsys.RDP.RadarTrack()
                {
                    ActualAircraft = networkPilot,
                    AboveTransition = true,
                    LatLong = new(dto.Latitude.GetValueOrDefault(), dto.Longitude.GetValueOrDefault()),
                };

                
            }
        }
    }
}
