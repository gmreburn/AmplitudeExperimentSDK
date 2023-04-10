namespace AmplitudeTests
{
    using Amplitude;
    using Moq;
    using Moq.Protected;
    using System.Net;

    public class Tests
    {
        private ExperimentClient client;
        private Mock<HttpMessageHandler> handlerMock;

        [SetUp]
        public void Setup()
        {
            handlerMock = new Mock<HttpMessageHandler>();
            client = new ExperimentClient(new HttpClient(handlerMock.Object));

            // Enable this line for E2E test:
            // client = new ExperimentClient("client-YYYY", new ExperimentUser() { UserId = "test" });
        }

        [Test]
        public async Task Test_SingleFlag()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""example-flag-key"":{""key"":""on""}}"),
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var variants = await client.VariantAsync("example-flag-key");
            Assert.That(variants.First().Value, Is.EqualTo("on"));
        }

        [Test]
        public async Task Test_SingleFlagNotFound()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{}"),
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var variants = await client.VariantAsync("example-flag-key");
            Assert.That(variants.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task Test_SingleFlagWithAUser()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""example-flag-key"":{""key"":""on""}}"),
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var variants = await client.VariantAsync("example-flag-key", new ExperimentUser() { UserId = "test2" });
            Assert.That(variants.First().Value, Is.EqualTo("on"));
        }

        [Test]
        public async Task Test_MultipleFlags()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""example-flag-key1"":{""key"":""on""},""example-flag-key2"":{""key"":""on""},""example-flag-key3"":{""key"": ""on""}}"),
            };
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var variants = await client.VariantAsync();
            Assert.That(variants.Count(), Is.EqualTo(3));
        }
    }
}