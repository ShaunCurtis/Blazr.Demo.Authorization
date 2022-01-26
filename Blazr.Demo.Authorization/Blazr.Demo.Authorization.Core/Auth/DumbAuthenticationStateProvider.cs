/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Demo.Authorization.Core;

public class DumbAuthenticationStateProvider : AuthenticationStateProvider
{
    ClaimsPrincipal? _user;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_user ?? new ClaimsPrincipal()));

    public Task<AuthenticationState> ChangeIdentityAsync(string username)
    {
        _user = new ClaimsPrincipal(TestIdentities.GetIdentity(username));
        var task = this.GetAuthenticationStateAsync();
        this.NotifyAuthenticationStateChanged(task);
        return task;
    }
}

