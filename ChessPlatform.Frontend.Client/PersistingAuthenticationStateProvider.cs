using System.Security.Claims;
using ChessPlatform.Frontend.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChessPlatform.Frontend.Client;

public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> DefaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> _authenticationStateTask = DefaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        Console.WriteLine("Attempt client");
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            Console.WriteLine("No user info");
            return;
        }

        Claim[] claims = [
            new Claim(ClaimTypes.Email, userInfo.Email) ];

        _authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}