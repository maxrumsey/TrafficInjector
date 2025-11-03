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

            if (Network.IsConnected)
            {
                _ = Connected();
            }
        }

        public void RegisterEvents()
        {
            Network.Connected += Connected;
            _fetcher.AircraftReceived += AircraftDataReceived;
        }

        public void DeregisterEvents()
        {
            Network.Connected -= Connected;
        }

        private async void Connected(object sender, EventArgs e)
        {
            await Connected();
        }
        
        private async Task Connected()
        {
            try
            {
                await _host.StopAsync();
                throw new Exception("Stopping traffic injection due to Network Connection");
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "TrafficInjector");
            }
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
            DeregisterEvents();
            _mmi.ClearTracks();
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
