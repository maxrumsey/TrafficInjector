using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddTrafficInjector(this IServiceCollection services)
        {
            return
                services.AddSingleton<IFetcher, Fetcher>()
                    .AddSingleton<StateManager>()
                    .AddSingleton<vatSysAccessor>()
                    .AddSingleton<RadarTargetRepository>()
                    .AddSingleton<PendingDTORepository>()
                    .AddHttpClient()
                    .AddHostedService<BackgroundWorker>()
                    .AddHostedService<TimedService>();
        }
    }
}
