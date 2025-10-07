using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public class RadarTargetRepository
    {
        public List<RadarTarget> Targets { get; set; } = new();

        public RadarTarget AddOrUpdate(AircraftDTO dto)
        {
            var target = Targets.FirstOrDefault(t => t.Hex == dto.HexCode && t.Callsign == dto.Callsign);

            if (target is null)
            {
                target = new RadarTarget(dto);
                Targets.Add(target);
            }
            else
            {
                target.Update(dto);
            }

            return target;
        }
    }
}
