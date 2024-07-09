using Microsoft.Net.Http.Headers;

namespace ChessPlatform.Frontend.Server.Handlers;

public class CookieHandler : DelegatingHandler
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext == null)
            return await base.SendAsync(request, cancellationToken);
        
        var authenticationCookie = httpContext.Request.Cookies[".AspNetCore.Identity.Application"];
        if (!string.IsNullOrEmpty(authenticationCookie))
        {                
            request.Headers.Add("Cookie", new CookieHeaderValue(".AspNetCore.Identity.Application", authenticationCookie).ToString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
