/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components;

namespace Blazr.Demo.Authorization.Core;

public class AdminAreaAuthorizationRequirement : IAuthorizationRequirement { }

public class AdminAreaAuthorizationHandler : AuthorizationHandler<AdminAreaAuthorizationRequirement>
{
    private readonly NavigationManager _navigationManager;

    public AdminAreaAuthorizationHandler(NavigationManager navigationManager)
        => _navigationManager = navigationManager;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAreaAuthorizationRequirement requirement)
    {
        if (context.User.IsInRole("AdminRole"))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
