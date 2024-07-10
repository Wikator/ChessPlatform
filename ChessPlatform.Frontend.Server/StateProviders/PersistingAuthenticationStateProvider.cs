using System.Diagnostics;
using System.Security.Claims;
using ChessPlatform.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ChessPlatform.Frontend.Server.StateProviders;

public class PersistingAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private Task<AuthenticationState>? _authenticationStateTask;
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;
    private readonly IdentityOptions _options;

    public PersistingAuthenticationStateProvider(
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
        _state = persistentComponentState;
        AuthenticationStateChanged += OnAuthenticationStateChanged;
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    private async Task OnPersistingAsync()
    {
        Console.WriteLine("Attempt");
        
        if (_authenticationStateTask is null)
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");

        var authenticationState = await _authenticationStateTask;
        var principal = authenticationState.User;
        
        Console.WriteLine(principal.Identity is null);
        
        Console.WriteLine(principal.Identity?.IsAuthenticated);
        foreach (var claim in principal.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }

        if (principal.Identity?.IsAuthenticated == true)
        {
            Console.WriteLine("User authenticated");
            // var userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            

            if (email is not null && userId is not null)
            {
                Console.WriteLine("Persisting...");
                _state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    Id = userId,
                    Email = email
                });
            }
        }
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask = authenticationStateTask;
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _authenticationStateTask?.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        _subscription.Dispose();
    }
}