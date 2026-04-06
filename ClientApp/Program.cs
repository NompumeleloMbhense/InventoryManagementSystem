using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using ClientApp;
using ClientApp.Services;
using ClientApp.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// 1. Auth Services
builder.Services.AddScoped<AuthTokenService>();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

// 2. Configure HttpClient with Automatic JWT handling
builder.Services.AddHttpClient("ServerAPI", client => 
    client.BaseAddress = new Uri("http://localhost:5075/")) // Ensure this matches your Server port
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

// Supply the default HttpClient from the factory
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

// 3. Business Services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SupplierService>();


// 4. Auth State Provider
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();


await builder.Build().RunAsync();
