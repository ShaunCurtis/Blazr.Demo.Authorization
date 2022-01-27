/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core;

public record AppAuthFields
{
    public Guid OwnerId { get; init; }
    
    public Guid AssigneeId { get; init; }
}

