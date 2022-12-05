using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json.Serialization;

namespace WireMockShowcase.Api
{
    public class OAuthClientCredentialsAuthenticator : AuthenticatorBase
    {
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _scopes;

        public OAuthClientCredentialsAuthenticator(string baseUrl, string clientId, string clientSecret, string scopes)
            : base("")
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scopes = scopes;

        }
        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            Token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, Token);
        }

        private async Task<string> GetToken()
        {
            var options = new RestClientOptions(_baseUrl);
            using var client = new RestClient(options)
            {
                Authenticator = new HttpBasicAuthenticator(_clientId, _clientSecret),
            };

            var request = new RestRequest("connect/token")
                .AddParameter("grant_type", "client_credentials")
                .AddParameter("scope", _scopes);

            var response = await client.PostAsync<TokenResponse>(request);
            return $"{response!.TokenType} {response!.AccessToken}";
        }

        record TokenResponse
        {
            [JsonPropertyName("token_type")]
            public string TokenType { get; init; }

            [JsonPropertyName("access_token")]
            public string AccessToken { get; init; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; init; }
        }
    }
}
