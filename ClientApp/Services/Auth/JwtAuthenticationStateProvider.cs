using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ClientApp.Services;


/// </summary>
/// The official source of truth for whether a user is logged in or not.
/// Blazor asks:
/// Who the user is
/// Whether they are authenticated
/// What claims they have
/// </summary>

namespace ClientApp.Services.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthTokenService _tokenService;

        public JwtAuthenticationStateProvider(AuthTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // This method is called by the Blazor framework to get the current authentication state.
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenService.GetTokenAsync();

            if(string.IsNullOrWhiteSpace(token))
            {
                // Not logged in
                var anonymousUser = new ClaimsPrincipal(
                    new ClaimsIdentity()
                );
                return new AuthenticationState(anonymousUser);
            }

            // Logged in - build claims from token
            var claims = JwtParser.ParseClaimsFromJwt(token);

            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "jwt")
            );
            return new AuthenticationState(authenticatedUser);
        }

        // This method is called when the user logs in to update the authentication state.
        public void NotifyUserAuthentication(string token)
        {
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "jwt")
            );

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(authenticatedUser))
            );
        }

        // This method is called when the user logs out to update the authentication state.
        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(
                new ClaimsIdentity()
            );

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(anonymousUser))
            );
        }

    }
}
