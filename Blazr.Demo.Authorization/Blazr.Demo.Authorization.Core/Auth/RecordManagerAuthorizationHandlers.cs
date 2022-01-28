/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core;

public class RecordManagerAuthorizationRequirement : IAuthorizationRequirement { }

public class RecordOwnerManagerAuthorizationHandler : AuthorizationHandler<RecordManagerAuthorizationRequirement, object>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordManagerAuthorizationRequirement requirement, object data)
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

public class RecordManagerAuthorizationHandler : AuthorizationHandler<RecordManagerAuthorizationRequirement, object>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordManagerAuthorizationRequirement requirement, object data)
    {
        if (context.User.IsInRole(AppPolicies.AdminRole))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
