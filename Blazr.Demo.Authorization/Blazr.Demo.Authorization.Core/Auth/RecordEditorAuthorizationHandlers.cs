/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core;

public class RecordEditorAuthorizationRequirement : IAuthorizationRequirement { }

public class RecordOwnerEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, AppAuthFields data)
    {
        var entityId = context.User.GetIdentityId();
        if (entityId != Guid.Empty && entityId == data.OwnerId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class RecordEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, AppAuthFields data)
    {
        if (context.User.IsInRole(AppPolicies.UserRole) || context.User.IsInRole(AppPolicies.AdminRole))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
