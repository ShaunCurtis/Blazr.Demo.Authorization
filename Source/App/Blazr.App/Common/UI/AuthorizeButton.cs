/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public class AuthorizeButton : ComponentBase
{
    [Parameter] public bool Show { get; set; } = true;

    [Parameter] public bool Disabled { get; set; } = false;

    [Parameter] public string Policy { get; set; } = String.Empty;

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> ClickEvent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> SplatterAttributes { get; set; } = new Dictionary<string, object>();

    [CascadingParameter] public Task<AuthenticationState>? AuthTask { get; set; }

    [Inject] protected IAuthorizationService? _authorizationService { get; set; }
    protected IAuthorizationService AuthorizationService => _authorizationService!;

    protected CSSBuilder CssClass = new CSSBuilder("btn me-1");

    protected override void OnInitialized()
    {
        if (AuthTask is null)
            throw new Exception($"{this.GetType().FullName} must have access to cascading Paramater {nameof(AuthTask)}");
    }

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this.Show && await this.CheckPolicy())
        {
            CssClass.AddClassFromAttributes(SplatterAttributes);
            builder.OpenElement(0, "button");
            builder.AddMultipleAttributes(1, this.SplatterAttributes);
            builder.AddAttribute(1, "class", CssClass.Build());

            if (Disabled)
                builder.AddAttribute(4, "disabled");

            if (ClickEvent.HasDelegate)
                builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ClickEvent));

            builder.AddContent(6, ChildContent);
            builder.CloseElement();
        }
    }

    protected virtual async Task<bool> CheckPolicy()
    {
        var state = await AuthTask!;
        var result = await this.AuthorizationService.AuthorizeAsync(state.User, null, Policy);
        return result.Succeeded;
    }
}
