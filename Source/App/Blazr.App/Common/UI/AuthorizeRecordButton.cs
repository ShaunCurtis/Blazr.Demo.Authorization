/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public class AuthorizeRecordButton : AuthorizeButton
{
    [Parameter] public object? AuthFields { get; set; }

    protected override async Task<bool> CheckPolicy()
    {
        var state = await AuthTask!;
        var result = await this.AuthorizationService.AuthorizeAsync(state.User, AuthFields, Policy);
        return result.Succeeded;
    }
}
