using Moq;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TrafficInjector.Plugin;
using vatsys;
using static vatsys.Track;

namespace TrafficInjector.Tests
{
    public class StateManagerTests
    {
        [Fact]
        public void RemoveExpiredAircraft_RemovesOldAircraft_WithQuicktag()
        {
            var repo = new RadarTargetRepository();
            var target = repo.AddOrUpdate(GenerateDTO());

            target.LastUpdated = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            target.QuickTag = new(target, target.Callsign);
            target.Track = CreateDummyTrack(target);

            var iVatSysAccessorMock = new Mock<IVatSysAccessor>();
            iVatSysAccessorMock.Setup(x => x.RemoveQuickTag(target.QuickTag)).Verifiable();
            iVatSysAccessorMock.Setup(x => x.RemoveTrack(target)).Verifiable();

            var sut = new StateManager(null!, iVatSysAccessorMock.Object, repo, null!, new());

            sut.RemoveExpiredAircraft();

            iVatSysAccessorMock.Verify(x => x.RemoveQuickTag(target.QuickTag), Times.Once);
            iVatSysAccessorMock.Verify(x => x.RemoveTrack(target), Times.Once);
        }
        private AircraftDTO GenerateDTO()
        {
            return new()
            {
                Callsign = "JST123",
                HexCode = "ABC123",
                AltitudeBaroRaw = 12000,
                AltitudeGeom = 11800,
                Latitude = 0,
                Longitude = 0,
            };
        }

        private Track CreateDummyTrack(RadarTarget target)
        {
            var type = typeof(Track);
            return FormatterServices.GetUninitializedObject(type) as Track ?? throw new Exception("Failed to create Track");
        }
    }
}