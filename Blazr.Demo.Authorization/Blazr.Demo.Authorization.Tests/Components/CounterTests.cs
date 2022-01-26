/// =================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Demo.Authorization.Tests.Components
{
    public class CounterTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(23)]
        public void CounterComponentButtonClicks(int noOfClicks)
        {
            // Define
            using var ctx = new TestContext();
            var component = ctx.RenderComponent<Counter>();

            // Test
            var button = component.Find("button");
            for (int i = 1; i <= noOfClicks; i++)
            {
                button.Click();
            }

            // Assert:
            component.Find("p").MarkupMatches($"<p role=\"status\">Current count: {noOfClicks}</p>");
        }
    }
}
