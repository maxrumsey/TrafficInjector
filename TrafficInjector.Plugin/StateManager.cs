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
        private vatSysAccessor _mmi;
        private RadarTargetRepository _repo;

        public StateManager(Fetcher fetcher,
            vatSysAccessor MMI,
            RadarTargetRepository targets)
        {
            _mmi = MMI;
            _repo = targets;
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

                var target = _repo.AddOrUpdate(dto);

                if (target.Track is null)
                {
                    target.Track = _mmi.AddTrack(target);

                    RDP.AddQuickTag(target, new(target, target.Callsign));
                }
            }
        }

        public void RemoveExpiredAircraft()
        {
            var expired = _repo.GetAndRemoveExpired();

            foreach (var aircraft in expired)
            {
                if (aircraft.Track is not null)
                {
                    _mmi.RemoveTrack(aircraft);
                }
            }
        }
    }
}
