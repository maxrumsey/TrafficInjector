using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class BackgroundWorker : BackgroundService
    {
        private IFetcher _fetcher;

        public BackgroundWorker(IFetcher fetcher)
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
