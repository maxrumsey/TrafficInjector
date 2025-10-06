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

        public TimedService(Fetcher fetcher)
        {
            _fetcher = fetcher;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(FetchData, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
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
