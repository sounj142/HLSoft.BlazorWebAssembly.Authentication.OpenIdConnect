# HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
This is an OpenIdConnect library for Blazor WebAssembly base on oidc-client-js (https://github.com/IdentityModel/oidc-client-js). Actually, this library is a brief wrapper of oidc-client-js, created to make our life with Blazor WebAssembly Client easier, only minimum configuration, minimum code, no javascript. Sound good?


#Quick Start

## Create Blazor WebAssembly App
-	Open the sample solution 
-	In this solution, add a new Blazor Webassembly project, call it BlazorClient

## Configure port and install Nuget package to BlazorClient App
- In BlazorClient project, expand Properties, open file launchSettings.json, change the iisSettings section to something like bellow:

```sh
"iisSettings": {
  "windowsAuthentication": false,
  "anonymousAuthentication": true,
  "iisExpress": {
    "applicationUrl": "http://localhost:5005/",
    "sslPort": 0
  }
},
```

-	Add nuget HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect to project, notice that it’s prerelease. You can install it using Visual Studio or CLI

```sh
Install-Package HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect -Version 0.5.0-Preview1
```

## Configure authentication for BlazorClient app

### Program.cs
-	In Program.cs, call AddBlazoredOpenIdConnect() to config Oidc:
```csharp
public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddAuthorizationCore(options => { })
                .AddBlazoredOpenIdConnect(options =>
                {
                    options.Authority = "http://localhost:5000/";

                    options.ClientId = "Client.Code";
                    options.ResponseType = "code";

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("api");
                });

            await builder.Build().RunAsync();
        }
    }
```

### App.razor
-	In App.razor, wrap all elements within CascadingAuthenticationState component and replace RouteView with AuthorizeRouteView. App.razor content should look like bellow:

```html
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(Program).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    <h1>Sorry, you're not authorized to view this page.</h1>
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

### _Imports.razor
-	Open _Imports.razor and add these namespaces:

```csharp
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Authorization
@using HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
```

### wwwroot/index.html
-	Open wwwroot/index.html, add these script elements ( place it after blazor.webassembly.js script )

```html
<script src="/_content/HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect/oidc-client.min.js"></script>
<script src="/_content/HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect/app.js"></script>
```

### Shared/NavMenu.razor
-	Add inject command to import the required service
```csharp
@inject IAuthenticationService authenticationService
```
-	Replace “Fetch data” menu with this code, we will add login/out menu here
```html
<AuthorizeView>
    <Authorized>
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="fetchdata">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Fetch data
            </NavLink>
        </li>
        <li class="nav-item px-3">
            <NavLink class="nav-link" @onclick="authenticationService.SignOutAsync" href="javascript:void(0)">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Log out
            </NavLink>
        </li>
    </Authorized>
    <NotAuthorized>
        <li class="nav-item px-3">
            <NavLink class="nav-link" @onclick="authenticationService.SignInAsync" href="javascript:void(0)">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Log in
            </NavLink>
        </li>
    </NotAuthorized>
</AuthorizeView>
```

### Shared/MainLayout.razor
Replace the About link with this code to display the greeting:
```html
<div class="top-row px-4">
        <AuthorizeView>
            <Authorized>
                Hello @context.User.Identity.Name
            </Authorized>
            <NotAuthorized>
                Please login!
            </NotAuthorized>
        </AuthorizeView>    
</div>
```

### Pages/FetchData.razor:
-	Add Inject command and Authorize attribute at the top:
```csharp
@page "/fetchdata"
@inject IAuthenticationStateProvider authenticationStateProvider
@attribute [Authorize]
```
-	Edit OnInitializedAsync to use API endpoint instead of static json file
```csharp
protected override async Task OnInitializedAsync()
{
    var httpClient = await authenticationStateProvider.GetHttpClientAsync();
    forecasts = await httpClient.GetJsonAsync<WeatherForecast[]>("http://localhost:5001/WeatherForecast");
}
```

## Run the application
-	Run all three projects, you should have Identity Server on 5000, API on 5001 and Blazor App on 5005.
-	Test the Log in, log out feature in app. You can use these two “awesome” accounts




   [oidc-repo]: <https://github.com/IdentityModel/oidc-client-js>







   


