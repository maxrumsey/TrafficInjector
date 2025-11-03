using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class TimedService : IHostedService, IDisposable
    {
        private Timer? _fetchTimer;
        private Timer? _clearTimer;
        private readonly Fetcher _fetcher;
        private readonly StateManager _stateManager;

        public TimedService(Fetcher fetcher, StateManager stateManager)
        {
            _fetcher = fetcher;
            _stateManager = stateManager;
        }

        public void Dispose()
        {
            _fetchTimer?.Dispose();
            _clearTimer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _fetchTimer = new Timer(PushData, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            _clearTimer = new Timer(ClearData, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void ClearData(object? state)
        {
            try
            {
                _stateManager.RemoveExpiredAircraft();
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "Traffic Injector");
            }
        }


        private void PushData(object? state)
        {
            try
            {
                _stateManager.UpdateTracks();
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "Traffic Injector");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _fetchTimer?.Change(Timeout.Infinite, 0);
            _clearTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
