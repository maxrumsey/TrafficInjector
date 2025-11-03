using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficInjector.Plugin
{
    public class PendingDTORepository
    {
        private List<AircraftDTO> _dtos = new();

        public void AddOrUpdate(AircraftDTO dto)
        {
            lock (_dtos)
            {

                var foundDTOs = _dtos.Where(t => t.HexCode == dto.HexCode && t.Callsign == dto.Callsign).ToArray();
                
                foreach (var oldDto in foundDTOs)
                {
                    _dtos.Remove(oldDto);
                }

                _dtos.Add(dto);
            }
        }

        public AircraftDTO[] EmptyAndReturnPending()
        {
            lock (_dtos)
            {
                var dtos = _dtos.ToArray();
                _dtos.Clear();
                return dtos;
            }
        }
    }
}
