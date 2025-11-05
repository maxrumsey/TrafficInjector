using TrafficInjector.Plugin;

namespace TrafficInjector.Tests
{
    public class RadarTargetRepositoryTests
    {
        [Fact]
        public void AddOrUpdate_InsertsNewDTO()
        {
            var sut = new RadarTargetRepository();

            var dto = GenerateDummyDTO();

            sut.AddOrUpdate(dto);

            Assert.Single(sut.GetAllTargets(), x => x.Callsign == dto.Callsign);
        }

        [Fact]
        public void AddOrUpdate_UpdatesExistingDTO()
        {
            var sut = new RadarTargetRepository();

            var oldDTO = GenerateDummyDTO();
            var newDTO = GenerateDummyDTO();
            newDTO.GroundSpeed = 400;
            

            sut.AddOrUpdate(oldDTO);

            sut.AddOrUpdate(newDTO);

            Assert.Single(sut.GetAllTargets(), x => x.GroundSpeed == 400);
        }

        [Fact]
        public void RemoveExpiredTargets_OldRemoved()
        {
            var sut = new RadarTargetRepository();
            var dto = GenerateDummyDTO();
            var target = sut.AddOrUpdate(dto);

            target.LastUpdated = DateTime.Now.Subtract(Constants.TARGET_EXPIRY).Subtract(TimeSpan.FromSeconds(1));

            var expiredTargets = sut.GetAndRemoveExpired();

            Assert.Single(expiredTargets, target);
            Assert.Empty(sut.GetAllTargets());
        }

        [Fact]
        public void RemoveExpiredTargets_FreshNotRemoved()
        {
            var sut = new RadarTargetRepository();
            var dto = GenerateDummyDTO();
            var target = sut.AddOrUpdate(dto);

            var expiredTargets = sut.GetAndRemoveExpired();

            Assert.Empty(expiredTargets);
            Assert.Single(sut.GetAllTargets(), target);
        }

        [Fact]
        public void SetCoastingStatus()
        {
            var sut = new RadarTargetRepository();
            
            var dto = GenerateDummyDTO();
            var target = sut.AddOrUpdate(dto);
            
            target.LastUpdated = DateTime.Now.Subtract(Constants.COASTING_TIMEOUT).Subtract(TimeSpan.FromSeconds(1));
            
            sut.SetCoastingStatus();
            
            Assert.True(target.Coasting);
            
            target.LastUpdated = DateTime.Now;
            
            sut.SetCoastingStatus();
            
            Assert.False(target.Coasting);
        }

        private AircraftDTO GenerateDummyDTO()
        {
            return new AircraftDTO()
            {
                HexCode = "ABC123",
                Callsign = "QFA1",
                AltitudeBaroRaw = 15000,
                AltitudeGeom = 15000,
            };
        }
    }
}