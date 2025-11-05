using TrafficInjector.Plugin;

namespace TrafficInjector.Tests
{
    public class PendingDTORepositoryTests
    {
        [Fact]
        public void AddOrUpdate_ShouldAddNewDto_WhenNotExists()
        {
            var repo = new PendingDTORepository();
            var dto = new AircraftDTO { HexCode = "ABC123", Callsign = "CALL1" };

            repo.AddOrUpdate(dto);
            var result = repo.EmptyAndReturnPending();

            Assert.Single(result);
            Assert.Equal("ABC123", result[0].HexCode);
            Assert.Equal("CALL1", result[0].Callsign);
        }

        [Fact]
        public void AddOrUpdate_ShouldReplaceExistingDto_WithSameHexAndCallsign()
        {
            var repo = new PendingDTORepository();
            var oldDto = new AircraftDTO { HexCode = "ABC123", Callsign = "CALL1" };
            var newDto = new AircraftDTO { HexCode = "ABC123", Callsign = "CALL1" };

            repo.AddOrUpdate(oldDto);
            repo.AddOrUpdate(newDto);

            var result = repo.EmptyAndReturnPending();

            Assert.Single(result);
            Assert.Same(newDto, result[0]); // Ensure the new object replaced the old
        }

        [Fact]
        public void AddOrUpdate_ShouldKeepDifferentDtos_WhenHexOrCallsignDiffers()
        {
            var repo = new PendingDTORepository();

            var dto1 = new AircraftDTO { HexCode = "HEX1", Callsign = "CALL1" };
            var dto2 = new AircraftDTO { HexCode = "HEX2", Callsign = "CALL2" };

            repo.AddOrUpdate(dto1);
            repo.AddOrUpdate(dto2);

            var result = repo.EmptyAndReturnPending();

            Assert.Equal(2, result.Length);
            Assert.Contains(result, x => x.HexCode == "HEX1" && x.Callsign == "CALL1");
            Assert.Contains(result, x => x.HexCode == "HEX2" && x.Callsign == "CALL2");
        }

        [Fact]
        public void EmptyAndReturnPending_ShouldClearRepository()
        {
            var repo = new PendingDTORepository();
            var dto = new AircraftDTO { HexCode = "HEX1", Callsign = "CALL1" };

            repo.AddOrUpdate(dto);
            var firstCall = repo.EmptyAndReturnPending();
            var secondCall = repo.EmptyAndReturnPending();

            Assert.Single(firstCall);
            Assert.Empty(secondCall);
        }

        [Fact]
        public void AddOrUpdate_MultipleUpdates_ShouldKeepLatestForSameHexAndCallsign()
        {
            var repo = new PendingDTORepository();
            var dto1 = new AircraftDTO { HexCode = "HEX1", Callsign = "CALL1" };
            var dto2 = new AircraftDTO { HexCode = "HEX1", Callsign = "CALL1" };
            var dto3 = new AircraftDTO { HexCode = "HEX1", Callsign = "CALL1" };

            repo.AddOrUpdate(dto1);
            repo.AddOrUpdate(dto2);
            repo.AddOrUpdate(dto3);

            var result = repo.EmptyAndReturnPending();

            Assert.Single(result);
            Assert.Same(dto3, result[0]);
        }
    }
}