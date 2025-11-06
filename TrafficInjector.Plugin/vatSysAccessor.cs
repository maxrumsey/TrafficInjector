using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class VatSysAccessor : IVatSysAccessor
    {
        private MethodInfo _addTrackMethod;

        private MethodInfo _removeTrackMethod;

        private MethodInfo _visCentreGetter;

        private MethodInfo _clearTracksMethod;

        public VatSysAccessor()
        {
            _addTrackMethod = typeof(MMI).GetMethod("AddTrack",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { typeof(object), typeof(Track.TrackTypes) }, null) ?? throw new Exception("Could not fetch AddTrack.");

            _removeTrackMethod = typeof(MMI).GetMethod("RemoveTrack",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { typeof(object) }, null) ?? throw new Exception("Could not fetch RemoveTrack.");

            _visCentreGetter = typeof(Network).GetProperty("VisibilityCenters", BindingFlags.NonPublic | BindingFlags.Instance)?.GetGetMethod(true) ?? throw new Exception("Could not fetch Network.VisibilityCentres getter.");

            _clearTracksMethod = typeof(MMI).GetMethod("ClearTracks",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { }, null) ?? throw new Exception("Could not fetch AddTrack.");
        }

        public Track AddTrack(RDP.RadarTrack rt)
        {
            var track = _addTrackMethod.Invoke(null, new object[] { rt, Track.TrackTypes.TRACK_TYPE_RADAR });

            return (Track)track;
        }

        public void RemoveTrack(RDP.RadarTrack rt)
        {
            _removeTrackMethod.Invoke(null, new object[] { rt });
        }

        public void ClearTracks()
        {
            _clearTracksMethod.Invoke(null, []);
        }

        public IList<Coordinate> GetVisCentres()
        {
            var visCentres = _visCentreGetter.Invoke(GetNetworkInstance(), []) ?? throw new Exception("Could not get vis centres");

            return (IList<Coordinate>)visCentres;
        }

        private Network GetNetworkInstance()
        {
            var inst = typeof(Network).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) ?? throw new Exception("Could not fetch Network.Instance.");

            return (Network)inst;
        }
    }
}
