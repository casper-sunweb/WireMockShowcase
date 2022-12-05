using RestSharp;

namespace WireMockShowcase.Api
{
    public interface IBookingDataApiClient
    {
        Task<IEnumerable<BookingDataResponse>> RequestBookingDataAsync(BookingDataRequest bookingDataRequest);
    }

    public class BookingDataApiClient : IBookingDataApiClient, IDisposable
    {
        private readonly RestClient _client;

        public BookingDataApiClient(BookingDataApiSettings settings)
        {
            _client = new RestClient(settings.BaseUrl);
        }

        public BookingDataApiClient(BookingDataApiSettings settings, ClientCredentialSettings clientCredentialSettings)
        {
            _client = new RestClient(settings.BaseUrl)
            {
                Authenticator = new OAuthClientCredentialsAuthenticator(clientCredentialSettings.BaseUrl, clientCredentialSettings.ClientId, clientCredentialSettings.ClientSecret, scopes: "api_fligbook")
            };
        }

        public async Task<IEnumerable<BookingDataResponse>> RequestBookingDataAsync(BookingDataRequest bookingDataRequest)
        {
            var request = new RestRequest("BookingData/Request")
               .AddJsonBody(bookingDataRequest);

            var response = await _client.PostAsync<IEnumerable<BookingDataResponse>>(request);

            return response ?? Enumerable.Empty<BookingDataResponse>();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }

    public class BookingDataApiSettings
    {
        public string BaseUrl { get; init; }
    }

    public class ClientCredentialSettings
    {
        public string BaseUrl { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }

    public record BookingDataRequest
    {
        public string AirlineCode { get; init; }
        public string AirportCode { get; init; }
        public DateTime FlightDateFrom { get; init; }
        public DateTime FlightDateTill { get; init; }

    }

    public record BookingDataResponse
    {
        public int BookingNumber { get; init; }
    }

}
