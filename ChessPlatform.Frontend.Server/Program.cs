using System.Security.Claims;
using ChessPlatform.Frontend.Client;
using ChessPlatform.Frontend.Client.Services;
using ChessPlatform.Frontend.Server.Components;
using ChessPlatform.Frontend.Server.Handlers;
using ChessPlatform.Frontend.Server.StateProviders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddServiceDiscovery();
builder.Services.AddSingleton<SignalRChessService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddTransient<CookieHandler>();

builder.Services.AddHttpClient("AuthorizedClient", client =>
    {
        client.BaseAddress = new Uri("http://backend");
    })
    .AddHttpMessageHandler<CookieHandler>()
    .AddServiceDiscovery();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("AuthorizedClient");
});

builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/server-login",  async (HttpClient client, HttpContext httpContext) =>
{
    if (!httpContext.Request.Cookies.TryGetValue(".AspNetCore.Identity.Application", out var authCookie))
        return Results.Redirect("/login");
    
    var request = new HttpRequestMessage(HttpMethod.Get, "account/email");
    request.Headers.Add("Cookie", authCookie);
    var response = client.SendAsync(request).Result;

    if (!response.IsSuccessStatusCode)
        return Results.Redirect("/login");

    var claims = new List<Claim>
    {
        new(ClaimTypes.Email, await response.Content.ReadAsStringAsync())
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    
    var authProperties = new AuthenticationProperties
    {
        IsPersistent = true
    };

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity), authProperties);

    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ChessBoard).Assembly);

app.Run();