/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Core;

public static class AppPolicies
{
    public const string AdminRole = "AdminRole";
    public const string UserRole = "UserRole";
    public const string VisitorRole = "VisitorRole";

    public const string IsEditorPolicy = "IsEditorPolicy";
    public const string IsViewerPolicy = "IsViewerPolicy";
    public const string IsManagerPolicy = "IsManagerPolicy";
    public const string IsAdminPolicy = "IsAdminPolicy";
    public const string IsUserPolicy = "IsUserPolicy";
    public const string IsVisitor = "IsVisitor";

    public static AuthorizationPolicy IsAdminAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AdminRole)
        .Build();

    public static AuthorizationPolicy IsUserAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AdminRole, UserRole)
        .Build();

    public static AuthorizationPolicy IsVisitorAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AdminRole, UserRole, VisitorRole)
        .Build();

    public static AuthorizationPolicy IsEditorAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new RecordEditorAuthorizationRequirement())
        .Build();
    
    public static AuthorizationPolicy IsManagerAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new RecordManagerAuthorizationRequirement())
        .Build();

    public static AuthorizationPolicy IsViewerAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    public static Dictionary<string, AuthorizationPolicy> Policies
    {
        get
        {
            var policies = new Dictionary<string, AuthorizationPolicy>();

            policies.Add(IsAdminPolicy, IsAdminAuthorizationPolicy);
            policies.Add(IsUserPolicy, IsUserAuthorizationPolicy);
            policies.Add(IsVisitor, IsVisitorAuthorizationPolicy);

            policies.Add(IsManagerPolicy, IsManagerAuthorizationPolicy);
            policies.Add(IsEditorPolicy, IsEditorAuthorizationPolicy);
            policies.Add(IsViewerPolicy, IsViewerAuthorizationPolicy);
            return policies;
        }
    }

    public static void AddAppPolicyServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, RecordOwnerEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordManagerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordOwnerManagerAuthorizationHandler>();
    }
}

