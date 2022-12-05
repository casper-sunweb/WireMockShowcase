using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMockShowcase.Api.Tests
{
    [TestClass]
    public class BookingDataApiClientTests
    {
        [TestMethod]
        public async Task When_booking_data_is_requested_Then_external_API_is_called()
        {
            // Arrange
            #region WireMock setup

            var wiremock = WireMockServer.Start();
            var baseUrl = wiremock.Url ?? string.Empty;

            wiremock
                .Given(Request.Create()
                    .WithPath("/BookingData/Request")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new List<BookingDataResponse>() { new BookingDataResponse() { BookingNumber = 5 } }));

            #endregion

            var settings = new BookingDataApiSettings()
            {
                BaseUrl = baseUrl
            };

            var sut = new BookingDataApiClient(settings);

            var request = new BookingDataRequest()
            {
                AirlineCode = "HV",
                AirportCode = "AMS",
                FlightDateFrom = DateTime.Today,
                FlightDateTill = DateTime.Today.AddDays(1),
            };

            // Act
            var response = await sut.RequestBookingDataAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Count());
            Assert.AreEqual(new BookingDataResponse() {  BookingNumber = 5 }, response.First());
        }

        [TestMethod]
        public async Task When_booking_data_is_request_Then_correct_authorization_header_is_sent()
        {
            // Arrange
            #region WireMock setup

            var wiremock = WireMockServer.Start();
            var baseUrl = wiremock.Url ?? string.Empty;

            wiremock
                .Given(Request.Create()
                    .WithPath("/BookingData/Request")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new List<BookingDataResponse>() { new BookingDataResponse() { BookingNumber = 5 } }));

            wiremock
                .Given(Request.Create()
                    .WithPath("/connect/token")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        access_token = "jwt_token_data",
                        expires_in = 3600,
                        token_type = "Bearer",
                    }));

            #endregion

            var settings = new BookingDataApiSettings()
            {
                BaseUrl = baseUrl
            };

            var credentials = new ClientCredentialSettings()
            {
                BaseUrl = baseUrl,
                ClientId = "a_client",
                ClientSecret = "a_secret"
            };

            var sut = new BookingDataApiClient(settings, credentials);

            var request = new BookingDataRequest()
            {
                AirlineCode = "HV",
                AirportCode = "AMS",
                FlightDateFrom = DateTime.Today,
                FlightDateTill = DateTime.Today.AddDays(1),
            };

            // Act
            await sut.RequestBookingDataAsync(request);

            // Assert   
            var expectedHeaderValue = $"Bearer jwt_token_data";
            
            var entries = wiremock.FindLogEntries(Request.Create().WithPath("/BookingData/Request"));
            Assert.AreEqual(1, entries.Count());

            var actualRequest = entries.First();
            Assert.IsTrue(actualRequest.RequestMessage?.Headers?.Any(x => x.Key == "Authorization" && x.Value.First() == expectedHeaderValue));
        }
    }
}