using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using vatsys;
using vatsys.Plugin;

namespace TrafficInjector.Plugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        private IHost? PluginHost;
        private bool _isRunning = false;

        public Plugin()
        {
            AddMenuItem();
        }

        private void AddMenuItem()
        {
            var menuItem = new CustomToolStripMenuItem(CustomToolStripMenuItemWindowType.Main, CustomToolStripMenuItemCategory.Settings, new ToolStripMenuItem("Traffic Injector"));
            menuItem.Item.Click += ToggleActive;
            MMI.AddCustomMenuItem(menuItem);
        }

        private async void ToggleActive(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                await Start();
            }
            else
            {
                await Stop();
            }
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
                var builder = Host.CreateApplicationBuilder();

                builder.Services.AddTrafficInjector();
                builder.Logging.AddConsole();

                PluginHost = builder.Build();

                await PluginHost.StartAsync();
                
                PluginHost.Services.GetRequiredService<StateManager>().RegisterEvents();
                _isRunning = true;
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
                if (PluginHost is null)
                {
                    return;
                }

                await PluginHost.StopAsync();
                PluginHost.Dispose();
                PluginHost = null;

                _isRunning = false;
            }
            catch (Exception ex)
            {
                Errors.Add(ex, "Traffic Injector");
            }
        }
    }
}
