using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using ClientApp;
using ClientApp.Services;
using ClientApp.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5075/") // API base address
});

// Register ProductService and SupplierService
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SupplierService>();

// Register the AuthTokenService
builder.Services.AddScoped<AuthTokenService>();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5075/");
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("ServerAPI"));


builder.Services.AddAuthorizationCore();

// Whenever something asks for AuthenticationStateProvider,
// give it an instance of JwtAuthenticationStateProvider.
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

await builder.Build().RunAsync();
