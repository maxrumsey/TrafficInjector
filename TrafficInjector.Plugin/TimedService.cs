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
        private Timer? _timer;
        private readonly Fetcher _fetcher;
        private readonly StateManager _stateManager;

        public TimedService(Fetcher fetcher, StateManager stateManager)
        {
            _fetcher = fetcher;
            _stateManager = stateManager;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(FetchData, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
            _timer = new Timer(ClearData, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1));
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

        private async void FetchData(object? state)
        {
            try
            {
                await _fetcher.FetchAndFireDTOs();
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "Traffic Injector");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
