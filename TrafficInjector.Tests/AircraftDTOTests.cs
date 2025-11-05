using System.Runtime.CompilerServices;
using TrafficInjector.Plugin;

namespace TrafficInjector.Tests
{
    public class AircraftDTOTests
    {
        [Fact]
        public void AltitudeBaro()
        {
            var dto = new AircraftDTO
            {
                AltitudeBaroRaw = "ground",
            };

            Assert.Equal(0, dto.AltitudeBaro);

            dto.AltitudeBaroRaw = "1000";

            Assert.Equal(1000, dto.AltitudeBaro);

            dto.AltitudeBaroRaw = null;
            Assert.Equal(0, dto.AltitudeBaro);
        }

        [Fact]
        public void IsValid()
        {
            var dto = GenerateValidDTO();

            Assert.True(dto.IsValid());
        }

        [Fact]
        public void IsValid_NoHex()
        {
            var dto = GenerateValidDTO();
            dto.HexCode = null;

            Assert.False(dto.IsValid());
        }

        [Fact]
        public void IsValid_NoCallsign()
        {
            var dto = GenerateValidDTO();
            dto.Callsign = null;

            Assert.False(dto.IsValid());
        }

        [Fact]
        public void IsValid_NoAltGeom()
        {
            var dto = GenerateValidDTO();
            dto.AltitudeGeom = null;

            Assert.False(dto.IsValid());
        }

        [Fact]
        public void IsValid_NoLat()
        {
            var dto = GenerateValidDTO();
            dto.Latitude = null;

            Assert.False(dto.IsValid());
        }

        [Fact]
        public void IsValid_NoLongitude()
        {
            var dto = GenerateValidDTO();
            dto.Longitude = null;

            Assert.False(dto.IsValid());
        }

        private AircraftDTO GenerateValidDTO()
        {
            return new AircraftDTO
            {
                HexCode = "ABC123",
                Callsign = "QFA1",
                AltitudeGeom = 30000,
                Latitude = 0f,
                Longitude = 0f,
            }
            ;
        }
    }
}