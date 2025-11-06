using Moq;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using TrafficInjector.Plugin;
using vatsys;

namespace TrafficInjector.Tests
{
    public class BackgroundWorkerTests
    {
        [Fact]
        public async void CheckFetcherIsCalled()
        {
            var mock = new Mock<IFetcher>();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                tokenSource.Cancel();
                Assert.Fail("Failed to halt in time.");
            });

            mock.Setup(f => f.FetchForAllVisCentres()).Returns(() =>
            {
                tokenSource.Cancel();
                return Task.CompletedTask;
            }).Verifiable();

            var sut = new Plugin.BackgroundWorker(mock.Object);

            await sut.StartAsync(token);

            mock.Verify(f => f.FetchForAllVisCentres(), Times.Once);
        }

        [Fact]
        public async void CheckFetcher_LogsError()
        {
            var mock = new Mock<IFetcher>();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Errors.RemoveAll();
            var ex = new Exception("Test Exception");
            mock.Setup(f => f.FetchForAllVisCentres()).Returns(() =>
            {
                tokenSource.Cancel();
                throw ex;
            }).Verifiable();

            var sut = new Plugin.BackgroundWorker(mock.Object);

            await sut.StartAsync(token);

            mock.Verify(f => f.FetchForAllVisCentres(), Times.Once);

            Assert.Single(Errors.Current, e => e == ex);
        }
    }
}