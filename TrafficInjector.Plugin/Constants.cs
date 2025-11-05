using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public static class Constants
    {
        public static readonly TimeSpan COASTING_TIMEOUT = TimeSpan.FromSeconds(6);

        public static readonly TimeSpan TARGET_EXPIRY = TimeSpan.FromSeconds(15);
    }
}
