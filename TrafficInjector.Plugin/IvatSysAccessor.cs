using System.Collections.Generic;
using vatsys;
using static vatsys.RDP;

namespace TrafficInjector.Plugin
{
    public interface IVatSysAccessor
    {
        Track AddTrack(RDP.RadarTrack rt);
        void ClearTracks();
        IList<Coordinate> GetVisCentres();
        void RemoveTrack(RDP.RadarTrack rt);
        void AddQuickTag(RadarTrack rt, QuickTag qt);
        void RemoveQuickTag(QuickTag qt);
    }
}