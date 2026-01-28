using System.Net.Http.Headers;

namespace ClientApp.Services
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly AuthTokenService _tokenService;

        public JwtAuthorizationMessageHandler(AuthTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}