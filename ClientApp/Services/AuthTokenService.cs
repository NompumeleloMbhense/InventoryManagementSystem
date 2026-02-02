using Microsoft.JSInterop;


/// <summary>
/// Service for managing authentication tokens in local storage.
/// Provides methods to set, get, and remove tokens.
/// </summary>

namespace ClientApp.Services
{
    public class AuthTokenService
    {
        private readonly IJSRuntime _js;
        private const string TokenKey = "authToken";
        public event Action? AuthStateChanged;

        public AuthTokenService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task SetTokenAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            AuthStateChanged?.Invoke();
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
        }

        public async Task RemoveTokenAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            AuthStateChanged?.Invoke();
        }

    }
}