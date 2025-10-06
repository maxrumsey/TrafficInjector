using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using vatsys;
using vatsys.Plugin;

namespace TrafficInjector.Plugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        private IHost PluginHost;

        public Plugin()
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddTrafficInjector();
            builder.Logging.AddConsole();

            PluginHost = builder.Build();

            Start();
        }

        string IPlugin.Name => "Traffic Injector";

        void IPlugin.OnFDRUpdate(FDP2.FDR updated)
        {
        }

        void IPlugin.OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
        }

        async Task Start()
        {
            try
            {
                await PluginHost.StartAsync();
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "Traffic Injector");
            }
        }

        async Task Stop()
        {
            try
            {
                await PluginHost.StopAsync();
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "Traffic Injector");
            }
        }
    }
}
