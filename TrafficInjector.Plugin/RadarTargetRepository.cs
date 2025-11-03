using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public class RadarTargetRepository
    {
        private List<RadarTarget> _targets = new();

        public RadarTarget AddOrUpdate(AircraftDTO dto)
        {
            lock (_targets)
            {

                var target = _targets.FirstOrDefault(t => t.Hex == dto.HexCode && t.Callsign == dto.Callsign);

                if (target is null)
                {
                    target = new RadarTarget(dto);
                    _targets.Add(target);
                }
                else
                {
                    target.Update(dto);
                }

                return target;
            }
        }

        public RadarTarget[] GetAndRemoveExpired()
        {
            lock (_targets)
            {
                var targets = _targets.Where(x => x.LastUpdated <= DateTime.Now.Subtract(TimeSpan.FromSeconds(15)));

                _targets.RemoveAll(targets.Contains);

                return targets.ToArray();
            }
        }

        public void SetCoastingStatus()
        {
            lock (_targets)
            {
                foreach (var target in _targets)
                {
                    if (target.LastUpdated <= DateTime.Now.Subtract(TimeSpan.FromSeconds(6)))
                    {
                        target.Coasting = true;
                    }
                    else
                    {
                        target.Coasting = false;
                    }
                }
            }
        }
    }
}
