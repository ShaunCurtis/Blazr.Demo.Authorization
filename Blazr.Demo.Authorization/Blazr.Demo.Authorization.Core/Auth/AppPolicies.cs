/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core;

public static class AppPolicies
{
    public const string IsEditor = "IsEditor";
    public const string IsViewer = "IsViewer";

    public static AuthorizationPolicy IsEditorPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new RecordEditorAuthorizationRequirement())
        .Build();

    public static AuthorizationPolicy IsViewerPolicy
    => new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

    public static Dictionary<string, AuthorizationPolicy> Policies
    {
        get
        {
            var policies = new Dictionary<string, AuthorizationPolicy>();
            policies.Add(IsEditor, IsEditorPolicy);
            policies.Add(IsViewer, IsViewerPolicy);
            return policies;
        }
    }
}

