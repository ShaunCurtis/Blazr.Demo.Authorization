/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Template.Tests.ViewServices
{
    public partial class WeatherForecastViewServiceTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(55, 55)]
        [InlineData(233, 233)]
        public async void WeatherForecastViewServiceShouldGetWeatherForecastsAsync(int noOfRecords, int expectedCount)
        {
            // define
            var dataBrokerMock = new Mock<IWeatherForecastDataBroker>();
            WeatherForecastViewService? weatherForecastViewService = new WeatherForecastViewService(weatherForecastDataBroker: dataBrokerMock.Object);

            dataBrokerMock.Setup(item =>
                item.GetWeatherForecastsAsync())
               .Returns(this.GetWeatherForecastListAsync(noOfRecords));
            object? eventSender = null;
            object? eventargs = null;
            weatherForecastViewService.ListChanged += (sender, e) => { eventSender = sender; eventargs = e; };

            // test
            await weatherForecastViewService.GetForecastsAsync();

            // assert
            Assert.IsType<List<WeatherForecast>?>(weatherForecastViewService.Records);
            Assert.Equal(expectedCount, weatherForecastViewService.Records!.Count);
            Assert.IsType<List<WeatherForecast>?>(eventSender);
            Assert.IsType<EventArgs>(eventargs);
            dataBrokerMock.Verify(item => item.GetWeatherForecastsAsync(), Times.Once);
            dataBrokerMock.VerifyNoOtherCalls();
        }
    }
}
