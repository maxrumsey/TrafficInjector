using System.Runtime.CompilerServices;
using TrafficInjector.Plugin;

namespace TrafficInjector.Tests
{
    public class RadarTargetTests
    {
        [Fact]
        public void AltitudeAssignmentCorrect()
        {
            var dto = new AircraftDTO()
            {
                Callsign = "TEST123",
                HexCode = "ABCDEF",
                AltitudeGeom = 12000,
                AltitudeBaroRaw = 13000,
            };

            var sut = new RadarTarget(dto);

            Assert.Equal(13000, sut.CorrectedAltitude);

            dto.AltitudeGeom = 9000;
            sut.Update(dto);
            Assert.Equal(9000, sut.CorrectedAltitude);
        }

        [Fact]
        public void QuickTag_CFLSet()
        {
            var dto = new AircraftDTO()
            {
                Callsign = "TEST123",
                HexCode = "ABCDEF",
                AltitudeGeom = 12000,
                AltitudeBaroRaw = 13000,
                SelectedAltitude = 9000,
            };

            var sut = new RadarTarget(dto);
            sut.QuickTag = new(null, dto.Callsign);

            sut.Update(dto);

            Assert.Equal(dto.SelectedAltitude, sut.QuickTag.CFL);
        }
    }
}