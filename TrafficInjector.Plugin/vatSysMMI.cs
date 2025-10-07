using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vatsys;

namespace TrafficInjector.Plugin
{
    public class vatSysMMI
    {
        private MethodInfo _addTrackMethod;

        private MethodInfo _removeTrackMethod;
        
        public vatSysMMI()
        {
            _addTrackMethod = typeof(MMI).GetMethod("AddTrack",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { typeof(object), typeof(Track.TrackTypes) }, null) ?? throw new Exception("Could not fetch AddTrack.");

            _removeTrackMethod = typeof(MMI).GetMethod("RemoveTrack",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null,
                new Type[] { typeof(object) }, null) ?? throw new Exception("Could not fetch RemoveTrack.");
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
    }
}
