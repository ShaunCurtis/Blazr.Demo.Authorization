/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core;

public class RecordEditorAuthorizationRequirement : IAuthorizationRequirement { }

public class RecordOwnerAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, object>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, object data)
    {
        var entityId = context.User.GetIdentityId();
        if (data is not null && data is AppAuthFields)
        {
            var appFields = data as AppAuthFields;
            if (entityId != Guid.Empty && entityId == appFields!.OwnerId)
                context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

public class RecordEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, object>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, object data)
    {
        if (context.User.IsInRole(StandardPolicies.Admin))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
