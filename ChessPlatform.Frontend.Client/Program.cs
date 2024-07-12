using ChessPlatform.Frontend.Client;
using ChessPlatform.Frontend.Client.Configuration;
using ChessPlatform.Frontend.Client.Handlers;
using ChessPlatform.Frontend.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<CookieHandler>();
// builder.Services.AddServiceDiscovery();

builder.Services.AddHttpClient("AuthorizedClient", client =>
    {
        client.BaseAddress = new Uri("http://localhost:5133");
    })
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("AuthorizedClient");
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


builder.Services.AddSingleton<SignalRChessService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// and this
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

await builder.Build().RunAsync();