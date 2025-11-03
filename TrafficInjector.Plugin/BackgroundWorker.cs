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
    internal class BackgroundWorker : BackgroundService
    {
        private Fetcher _fetcher;

        public BackgroundWorker(Fetcher fetcher)
        {
            _fetcher = fetcher;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _fetcher.FetchForAllVisCentres();
                }
                catch (Exception ex)
                {
                    Errors.Add(ex, "Traffic Injector");
                }
            }
        }
    }
}
