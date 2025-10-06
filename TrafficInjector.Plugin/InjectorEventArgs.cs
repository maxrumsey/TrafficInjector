using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public class InjectorEventArgs<T> : EventArgs
    {
        public T? Data { get; set; }
    }
}
