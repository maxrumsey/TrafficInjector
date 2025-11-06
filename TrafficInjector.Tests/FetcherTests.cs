using Moq;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TrafficInjector.Plugin;

namespace TrafficInjector.Tests
{
    public class FetcherTests
    {
        [Fact]
        public async Task FetchForAllVisCentres_ReturnsExpect()
        {
            List<AircraftDTO> returns = [];

            var mockVatsys = new Mock<IVatSysAccessor>();
            mockVatsys.Setup(x => x.GetVisCentres()).Returns([ new() ]);

            var dto = GenerateDTO();

            var httpClient = GenerateHttpClient([dto]);

            var sut = new Fetcher(httpClient, null!, mockVatsys.Object);
            
            sut.AircraftReceived += (s, e) =>
            {
                Assert.NotNull(e.Data);

                returns.AddRange(e.Data);
            };

            await sut.FetchForAllVisCentres();
            
            Assert.Single(returns, x => x.Callsign == dto.Callsign && x.HexCode == dto.HexCode);
        }

        [Fact]
        public async Task FetchForAllVisCentres_ReturnsNilIfNoValid()
        {
            List<AircraftDTO> returns = [];

            var mockVatsys = new Mock<IVatSysAccessor>();
            mockVatsys.Setup(x => x.GetVisCentres()).Returns([new()]);

            var dto = GenerateDTO();
            dto.Latitude = null; // Invalid DTO

            var httpClient = GenerateHttpClient([dto]);

            var sut = new Fetcher(httpClient, null!, mockVatsys.Object);

            sut.AircraftReceived += (s, e) =>
            {
                Assert.NotNull(e.Data);

                returns.AddRange(e.Data);
            };

            await sut.FetchForAllVisCentres();

            Assert.Empty(returns);
        }

        [Fact]
        public async Task FetchForAllVisCentres_ReturnsNilIfNoDTOsReturned()
        {
            List<AircraftDTO> returns = [];

            var mockVatsys = new Mock<IVatSysAccessor>();
            mockVatsys.Setup(x => x.GetVisCentres()).Returns([new()]);

            var httpClient = GenerateHttpClient([]);

            var sut = new Fetcher(httpClient, null!, mockVatsys.Object);

            sut.AircraftReceived += (s, e) =>
            {
                Assert.NotNull(e.Data);

                returns.AddRange(e.Data);
            };

            await sut.FetchForAllVisCentres();

            Assert.Empty(returns);
        }

        [Fact]
        public async Task FetchForAllVisCentres_ReturnsNilIfNoVisCtrs()
        {
            List<AircraftDTO> returns = [];

            var mockVatsys = new Mock<IVatSysAccessor>();
            mockVatsys.Setup(x => x.GetVisCentres()).Returns([]);

            var dto = GenerateDTO();
            dto.Latitude = null; // Invalid DTO

            var httpClient = GenerateHttpClient([dto]);

            var sut = new Fetcher(httpClient, null!, mockVatsys.Object);

            sut.AircraftReceived += (s, e) =>
            {
                Assert.NotNull(e.Data);

                returns.AddRange(e.Data);
            };

            await sut.FetchForAllVisCentres();

            Assert.Empty(returns);
        }

        private string CreateDummyResponse(AircraftDTO[]? dtos)
        {
            if (dtos is null)
            {
                return "{\"aircraft\":{}}";
            }    

            return JsonSerializer.Serialize(new {
                aircraft = dtos,
            });
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

        private HttpClient GenerateHttpClient(AircraftDTO[]? dtos)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*")
                .Respond("application/json", CreateDummyResponse(dtos));
            return new HttpClient(mockHttp);
        }
    }
}