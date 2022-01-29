# Policy Based Authorization in Blazor

Many applications need more granular and complex authorization that the canned examples in many demos.  This article shows:

1. How use and manage policy based authorization in an application.
2. How to build and manage policies in static classes.
3. How to build complex resource based policy classes to authorize an identity against. 

## Repository and Demonstrator

The project associated with this article is available [here on Github](https://github.com/ShaunCurtis/Blazr.Demo.Authorization).  The project is based on my [Blazr.Demo](https://github.com/ShaunCurtis/Blazr.Demo) project template that implements basic clean design principles.

## Authentication and Ownership

Discussing Authorization is a little like putting the cart before the horse.  To authorize we need an authenticated identity.

For this article we use a very simple authenticator.

1. `VerySimpleAuthenticationProvider` returns a standard `IndentityPrincipal` instance with a Guid security Id and a role.
2. `UserDisplay` is the equivalent to a log in page.  It's a simple select in the top `NavBar` that makes changing identity simple and quick.

The record set we use is the classic weather forecast.  I've added an `OwnerId` to the record so we can allow record owners to edit and delete their own records.

The code for the authenticator, component and weather forecast record is in the bottom of this article.

The image below shows the Identity select and the `FetchData` page for the user "Visitor-2".

![App View](./images/App-View.png)

## Authorization

Before delving into the detail, let's look at the end result.

1. An anonymous user can view the list, but that's all.

![App View](./images/Anonymous.png)

2. Users can edit or view any record, but not delete one.

![App View](./images/User.png)

3. Admins you can Edit/View/Delete all records.

![App View](./images/Admin.png)

4. Visitors can view all the records, but only Edit/Delete your own.

![App View](./images/Visitor.png)

5. Applying authorization to a backend service.  We may show the "Add Record" to the identity, but the service applies a `User/Admin` only policy. So a visitor can click the "Add Record" button , but the backend refuses to add the record.  In the demo I show a simple message.

![App View](./images/Visitor-Add.png)

## Authorization Button

In the UI we normally click buttons to do things.  I've built an `AuthorizeButton` component to encapsulate this.  There are two versions:

1. `AuthorizeButton` requires a policy.  This is the Add button at the top of the page.  We're just encapsulating specific `AuthorizeView` code into the button context.

```html
<AuthorizeButton Policy=@AppPolicies.IsVisitor class="btn btn-success" @onclick="AddRecord">Add Record</AuthorizeButton>
```
2. `AuthorizeRecordButton` requires a policy and an `AppAuthFields` object built from a specific record.  This is the Edit button that appears in each row and uses the row record to populate the `AppAuthFields` instance.

```html
<AuthorizeRecordButton Policy=@AppPolicies.IsEditorPolicy AuthFields="this.GetAuthFields(forecast)" type="button" class="btn-sm btn-primary" ClickEvent="() => this.EditRecord(forecast.Id)">Edit</AuthorizeRecordButton>
```

`AppAuthFields` provides a generic method to pass specific information into the authorization process.  We can add more fields to it as required.  We'll see how it works later.

```csharp
public record AppAuthFields
{
    public Guid OwnerId { get; init; }
}
```


`AuthorizeButton` looks like this.  Much of the code is pretty standard component fare, so I'll concentrate on just the authorization bit.

`OnInitialized` checks to ensure we have a `AuthTask` cascade i.e. we have the upstream authentication and authorization configured.  If not then it throws an exception.

`BuildRenderTree` calls `CheckPolicy()` to check if it should render the button.

`CheckPolicy` gets the authentication state and then calls `AuthorizeAsync` on the injected `IAuthorizationService`, passing in the `IdentityPrincipal` of the logging in identity, a null for the resource object (we'll come to resource objects shortly), and the policy name to apply.  If the result is `success`, it displays the button.

```csharp
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

```

`AuthorizeRecordButton` extends `AuthorizeButton`.  It passes an `AppAuthFields` obect to `AuthorizeAsync` as the resource object.  We'll see how the `AuthorizationService` uses this shortly.

```csharp
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
```

### Policies

It's important to understand the difference between a policy name and a policy object.  The Authorization service holds a map of policy names to policy objects.  When we provide a "policy" to an authorization component or call `AuthorizeAsync` on the `IAuthorizationService` we are providing the policy name.  It maps and uses the policy object associated with the policy name. The map is defined when the authorization services are set up in the application services collection in `Program`/`StartUp`.

Policy objects are defined as instances of the `AuthorizationPolicy` class and built using an `AuthorizationPolicyBuilder` instance.

The application defines a static `AppPolicies` class to hold and manage all the policy code.

1. Defines a set of nomenclature constants for role and policy string names.
2. Builds out a set of policy objects using the `AuthorizationPolicyBuilder`.
3. Defines a dictionary of policy names to policy objects to load into `AuthorizationService`.
4. Defines an `IServiceCollection` extension method to add all the `IAuthorizationHandlers` required by the policies.

The constants:

```csharp
public static class StandardPolicies
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
```

The basic Admin/User/Visitor site basded policy objects that check an identity is logged in and in one or more roles.

Note that for user to pass a policy they must satisfy all the requirements (an AND in logic terms).
  
```csharp
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

```

The record based policies.  These use requirements, defined in an `IAuthorizationRequirement` list.  We'll look at  `RecordEditorAuthorizationRequirement` and `RecordManagerAuthorizationRequirement` shortly.

```csharp

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
```

The `Policies` dictionary provides a convenient mechanism for defining and managing the application policies  `AuthorizationService` uses.

```csharp
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
```

Finally an `IServiceCollection` extension to add the policy handler services.  More on these below.

```csharp
    public static void AddAppPolicyServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, RecordOwnerEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordManagerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordOwnerManagerAuthorizationHandler>();
    }
```

Using the `AppPolicies` class, we can now define our services in `Program` or an application `IServiceCollection` extension method like this:

```csharp
services.AddScoped<AuthenticationStateProvider, VerySimpleAuthenticationStateProvider>();
services.AddAppPolicyServices();
services.AddAuthorization(config =>
{
    foreach (var policy in AppPolicies.Policies)
    {
        config.AddPolicy(policy.Key, policy.Value);
    }
});
```

### Authorization Requrements

Our policy object requirements are defined as classes implementing `IAuthorizationRequirement`.  `IAuthorizationRequirement` classes are empty reference classes.  We define two:

1. `RecordEditorAuthorizationRequirement` defines a record editor.
2.  `RecordManagerAuthorizationRequirement` defines a record manager.

```csharp
public class RecordEditorAuthorizationRequirement : IAuthorizationRequirement { }
public class RecordManagerAuthorizationRequirement : IAuthorizationRequirement { }
```

They are used by `IAuthorizationHandler` classes.

Requirements are assessed by the Policy on a OR logic basis.  An entity only needs satisfy one requirement of a collection to pass authorization.

### Authorization Handlers

Authorization Handlers implement `IAuthorizationHandlers` and inherit from the  `AuthorizationHandler` base class.  The base class defines two generics:

1. `TRequirement` - The `AuthorizationRequirement` class that this `AuthorizationHandler` applies to.  
2. `TResource`, the resource type for the resource we will provide.  This is the `resource` object passed in `AuthorizationService.AuthorizeAsync`.  We pass `AppAuthFields` instances which we populate from a record.

There are two mapped to each requirement.  The editor authorization handlers are shown below. 

1. `TRequirement` is `RecordEditorAuthorizationRequirement` 
2. `TResource` is `AppAuthFields`. 
3. `HandleRequirementAsync` is the method called to do the authorization.  

`RecordOwnerEditorAuthorizationHandler`:

1. Gets the user's Id from the `AuthorizationHandlerContext`.
2. Checks the Id against the `OwnerId` provided in the `AppAuthFields` instance
3. Sets success on the provided `AuthorizationHandlerContext` instance if the Ids match.

```csharp
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
```
`RecordEditorAuthorizationHandler` simply checks if the identity has the correct role.  We apply this check as an Authorization handler because it needs to be an OR.  The identity can be either the owner and/or have a User Role.

```csharp
public class RecordEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, AppAuthFields data)
    {
        if (context.User.IsInRole(AppPolicies.UserRole) || context.User.IsInRole(AppPolicies.AdminRole))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
```

`GetIdentityId` is an extension method on `ClaimsPrincipal`

```
public static Guid GetIdentityId(this ClaimsPrincipal principal)
{
    if (principal is not null)
    {
        var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
        if (claim is not null && Guid.TryParse(claim.Value, out Guid id))
            return id;
    }
    return Guid.Empty;
}
```

### So how does a policy work?

The services container defines a set of IAuthorizationHandlers.  Each is "mapped" by it's `TRequirement` to a specific requirement class.  A policy defines one or more requirements, and is mapped to a name in the `AuthorizationService`.  

So when we do this:

```csharp
var result = await this.AuthorizationService.AuthorizeAsync(state.User, AuthFields, Policy);
```
We are telling the authorization service to check the user against the policy with the following `AuthFields` instance.  The service maps the policy name to an actual policy and calls the policy.  It gets a list of all the `IAuthorizationHandler` instances in the services container that handle the defined requirement class.  It runs `HandleRequirementsAsync` on each: the order doesn't matter it's an OR.  It returns on the first success.

## Service based Authorization

In the application we use a `WeatherForecastViewService` to provide the data operations to `FetchData`.  You can see the full code in the project code.

To apply authorization we need to inject `AuthenticationStateProvider` and `AuthorizationService`.

```csharp
private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

private IAuthorizationService AuthorizationService { get; set; }

public string Message { get; set; } = string.Empty;

public WeatherForecastViewService(IWeatherForecastDataBroker weatherForecastDataBroker, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
{ 
    this.weatherForecastDataBroker = weatherForecastDataBroker;
    this.AuthenticationStateProvider = authenticationState;
    this.AuthorizationService = authorizationService;
}
```

And then use these in our CRUD methods:

```csharp
public async ValueTask AddRecord(WeatherForecast record)
{
    this.Message = string.Empty;
    var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
    var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, null, AppPolicies.IsUserPolicy);
    if (result.Succeeded)
    {
        await weatherForecastDataBroker!.AddForecastAsync(record);
        await GetForecastsAsync();
    }
    else
        this.Message = "That Ain't Allowed!";
}
```

# Appendix

## Authentication

This article uses a very simple authentication provider that allows quick switching of the authentication context.  There's no passwords involved!

### Test Identities

Step one is to provide some test identities.

First a class for our identities:

```csharp
public record TestIdentity
{
    public string Name { get; set; } = string.Empty;
    public Guid Id { get; set; } = Guid.Empty;
    public string Role { get; set; } = string.Empty;

    public Claim[] Claims
        => new[]{
            new Claim(ClaimTypes.Sid, this.Id.ToString()),
            new Claim(ClaimTypes.Name, this.Name),
            new Claim(ClaimTypes.Role, this.Role)
    };
}
```

The identities are held in a static class used throughout the application.

1. The primary method is `GetIdentity`.  Pass in a user name and get back a `ClaimsIdentity` object.  
2. The Guids used are simple made up ones: easy to reproduce in test weather records.
3. A role is set for each identity.

```csharp
public static class TestIdentities
{
    public const string Provider = "Dumb Provider";

    public static ClaimsIdentity GetIdentity(string userName)
    {
        var identity = identities.FirstOrDefault(item => item.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (identity == null)
            return new ClaimsIdentity();

        return new ClaimsIdentity(identity.Claims, Provider);
    }

    private static List<TestIdentity> identities = new List<TestIdentity>()
        {
            Visitor1Identity, 
            Visitor2Identity, 
            User1Identity, 
            User2Identity, 
            Admin1Identity, 
            Admin2Identity
        };

    public static List<string> GetTestIdentities()
    {
        var list = new List<string> { "None" };
        list.AddRange(identities.Select(identity => identity.Name!).ToList());
        return list;
    }

    public static Dictionary<Guid, string> TestIdentitiesDictionary()
    {
        var list = new Dictionary<Guid, string>();
        identities.ForEach(identity => list.Add(identity.Id, identity.Name));
        return list;
    }

    public static TestIdentity User1Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-100000000001"),
            Name = "User-1",
            Role = "UserRole"
        };

\\ .... more identities

    public static TestIdentity Admin2Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-300000000002"),
            Name = "Admin-2",
            Role = "AdminRole"
        };
}
```

The `AuthenticationStateProvider` looks like this.  `ChangeIdentityAsync` switches users based on the provided user name.

```csharp
public class VerySimpleAuthenticationStateProvider : AuthenticationStateProvider
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
```

Finally the "Log In Page".  In this case it's a simple select component in the top bar.  The select pulls the list of users from `TestIdentities` and calls `ChangeIdentityAsync` on the Authentication State Provider to switch users.

```csharp
@implements IDisposable
@namespace Blazr.Demo.Authorization.UI

<span class="me-2">Change User:</span>
<div class="w-25">
    <select id="userselect" class="form-control" @onchange="ChangeUser">
        @foreach (var value in TestIdentities.GetTestIdentities())
        {
            @if (value == _currentUserName)
            {
                 <option value="@value" selected>@value</option>
            }
            else
            {
                <option value="@value">@value</option>
            }
        }
    </select>
</div>
<span class="text-nowrap ms-3">
    <AuthorizeView>
        <Authorized>
            Hello, @(this.user.Identity?.Name ?? string.Empty)
        </Authorized>
        <NotAuthorized>
            Not Logged In
        </NotAuthorized>
    </AuthorizeView>
</span>

@code {
    [CascadingParameter] private Task<AuthenticationState>? authTask { get; set; }
    private Task<AuthenticationState> AuthTask => authTask!;

    [Inject] private AuthenticationStateProvider? authState { get; set; }
    private VerySimpleAuthenticationStateProvider AuthState => (VerySimpleAuthenticationStateProvider)authState!;

    private ClaimsPrincipal user = new ClaimsPrincipal();
    private string _currentUserName = "None";

    protected async override Task OnInitializedAsync()
    {
        var authState = await AuthTask;
        this.user = authState.User;
        AuthState.AuthenticationStateChanged += this.OnUserChanged;
    }

    private async Task ChangeUser(ChangeEventArgs e)
        =>  await AuthState.ChangeIdentityAsync(e.Value?.ToString() ?? string.Empty);

    private async void OnUserChanged(Task<AuthenticationState> state)
        => await this.GetUser(state);

    private async Task GetUser(Task<AuthenticationState> state)
    {
        var authState = await state;
        this.user = authState.User;
    }

    public void Dispose()
        => AuthState.AuthenticationStateChanged -= this.OnUserChanged;
    }
```

The component is used in `MainLayout`.

```csharp
@namespace Blazr.Demo.Authorization.UI
@inherits LayoutComponentBase

<PageTitle>Blazr.Demo</PageTitle>

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <UserBar />
            <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>
```

The control in action.

![User Select](./images/User-Select.png)

## Weather Forecast Ownership

The `OwnerID` field is added to`WeatherForecast` and populated with either the `Visitor-1` or `Visitor-2` Guid.

```csharp
public static List<WeatherForecast> CreateTestForecasts(int count)
{
    var list = new List<WeatherForecast>();
    var rng = new Random();
    for (var i = 1; i <= count; i++)
    {
        var c = rng.Next(1, 3);
        list.Add(new WeatherForecast
        {
            Id = Guid.NewGuid(),
            OwnerId = new Guid($"10000000-0000-0000-0000-20000000000{c}"),
            Date = DateTime.Now.AddDays(i),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        });
    }
    return list;
}
```
