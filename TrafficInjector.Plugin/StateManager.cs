using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class StateManager : IDisposable
    {
        private vatSysAccessor _mmi;
        private RadarTargetRepository _repo;
        private IHost _host;
        private Fetcher _fetcher;
        private PendingDTORepository _pendingDTOs;

        public StateManager(Fetcher fetcher,
            vatSysAccessor MMI,
            RadarTargetRepository targets,
            IHost host,
            PendingDTORepository pendingDTOs)
        {
            _mmi = MMI;
            _repo = targets;
            _host = host;
            _fetcher = fetcher;
            _pendingDTOs = pendingDTOs;
        }

        public void RegisterEvents()
        {
            _fetcher.AircraftReceived += AircraftDataReceived;
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

                _pendingDTOs.AddOrUpdate(dto);
            }
        }

        public void UpdateTracks()
        {
            var pending = _pendingDTOs.EmptyAndReturnPending();

            foreach (var dto in pending)
            {
                PushTrackToRDP(dto);
            }

            _repo.SetCoastingStatus();
        }

        private void PushTrackToRDP(AircraftDTO dto)
        {
            var target = _repo.AddOrUpdate(dto);

            if (target.Track is null)
            {
                target.Track = _mmi.AddTrack(target);

                RDP.AddQuickTag(target, new(target, target.Callsign));
            }
        }

        public void Dispose()
        {
            _mmi.ClearTracks();
        }

        public void RemoveExpiredAircraft()
        {
            var expired = _repo.GetAndRemoveExpired();

            foreach (var aircraft in expired)
            {
                if (aircraft.Track is not null)
                {
                    if (aircraft.QuickTag is not null)
                    {
                        RDP.RemoveQuickTag(aircraft.QuickTag);
                    }

                    _mmi.RemoveTrack(aircraft);

                }
            }
        }
    }
}
