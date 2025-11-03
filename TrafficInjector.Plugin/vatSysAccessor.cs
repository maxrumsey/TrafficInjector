using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class vatSysAccessor
    {
        private MethodInfo _addTrackMethod;

        private MethodInfo _removeTrackMethod;

        private MethodInfo _visCentreGetter;

        private MethodInfo _clearTracksMethod;

        public vatSysAccessor()
        {
            var netInstance = typeof(Network).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic, null,
                typeof(Network), null)?.GetValue(null) ?? throw new Exception("Could not fetch Network.Instance.");

            _addTrackMethod = typeof(MMI).GetMethod("AddTrack",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { typeof(object), typeof(Track.TrackTypes) }, null) ?? throw new Exception("Could not fetch AddTrack.");

            _removeTrackMethod = typeof(MMI).GetMethod("RemoveTrack",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { typeof(object) }, null) ?? throw new Exception("Could not fetch RemoveTrack.");

            _visCentreGetter = typeof(Network).GetProperty("VisibilityCentres", BindingFlags.NonPublic)?.GetGetMethod() ?? throw new Exception("Could not fetch Network.VisibilityCentres getter.");

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
    }
}
