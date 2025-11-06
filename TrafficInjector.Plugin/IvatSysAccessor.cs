using System.Collections.Generic;
using vatsys;

namespace TrafficInjector.Plugin
{
    public interface IVatSysAccessor
    {
        Track AddTrack(RDP.RadarTrack rt);
        void ClearTracks();
        IList<Coordinate> GetVisCentres();
        void RemoveTrack(RDP.RadarTrack rt);
    }
}