# HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect
This is an OpenIdConnect library for Blazor WebAssembly base on [oidc-client-js][oidc-repo]. Actually, this library is a brief wrapper of oidc-client-js, created to make our life with Blazor WebAssembly Client easier, only minimum configuration, minimum code, no javascript. If you want to know more detail, see [oidc-client document][oidc-doc]


# Quick Start
In this section, I am going to guide you the basic way to use this library step-by-step. To getting start faster, you should download this git repository. I will leverage the existing API and Identity Server in the examples folder. The remain thing we need to do is implement the Blazor Client App. Follow the steps bellow:

## 1. Create Blazor WebAssembly App
-	Open the HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect solution. Right click on Examples folder, choose Add new Project, Add a new Blazor WebAssembly project, call it BlazorClient
- Note: If you can't find Blazor WebAssembly template, run this CLI to install it:
```sh
dotnet new -i Microsoft.AspNetCore.Components.WebAssembly.Templates::3.2.0-preview2.20160.5
```

## 2. Configure port and install Nuget package to BlazorClient App
- In BlazorClient project, expand Properties, open launchSettings.json file, change the iisSettings section to something like bellow:

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
-	Add nuget HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect to BlazorClient project, notice that it’s prerelease. You can install it using Visual Studio or CLI
```sh
Install-Package HLSoft.BlazorWebAssembly.Authentication.OpenIdConnect -Version 0.6.1-Preview2
```

## 3. Configure authentication for BlazorClient app
### Program.cs
-	In Program.cs, call AddBlazoredOpenIdConnect() method to config OpenId Connect:
```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("app");
        builder.Services.AddBaseAddressHttpClient();
        builder.Services.AddOptions()
            .AddAuthorizationCore()
            .AddBlazoredOpenIdConnect(options =>
            {
                options.Authority = "http://localhost:5000/";

                options.ClientId = "Client.Code";
                options.ResponseType = "code";

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("api");
            })
            .AddHttpClient<WeatherForecastService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001/");
            });
        await builder.Build().RunAsync();
    }
}
```
If you examine the IdentityServer project, you should see that the config above is corresponding to this Client(file Config.cs):
```csharp
new Client
{
    ClientId = "Client.Code",
    ClientName = "Client.Code",

    AllowedGrantTypes = GrantTypes.Code,
    RequireClientSecret = false,

    RedirectUris = {
        "http://localhost:5005/signin-callback-oidc",
    },
    PostLogoutRedirectUris = { "http://localhost:5005/" },

    AllowedScopes =
    {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        IdentityServerConstants.StandardScopes.Email,
        "api"
    }
},
```


### WeatherForecastService.cs
- Add a service class to wrap the Weather Forecast API
```csharp
public class WeatherForecastService
{
    private HttpClient _httpClient;
    private readonly IAuthenticationStateProvider _stateProvider;

    public WeatherForecastService(HttpClient httpClient, IAuthenticationStateProvider stateProvider)
    {
        _httpClient = httpClient;
        _stateProvider = stateProvider;
    }

    public async Task<IList<WeatherForecast>> GetAll()
    {
        await _stateProvider.SetAuthorizationHeader(_httpClient);
        return await _httpClient.GetJsonAsync<IList<WeatherForecast>>("WeatherForecast");
    }
}

public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public string Summary { get; set; }

    public int TemperatureF { get; set; }
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
@inject WeatherForecastService weatherForecastService
@attribute [Authorize]
```
-	Edit @code section to use API endpoint instead of static json file
```csharp
@code {
    private IList<WeatherForecast> forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await weatherForecastService.GetAll();
    }
}
```

## 4. Run the application
-	Run all three projects API, IdentityServer and BlazorClient, you should have Identity Server on port 5000, API on 5001 and Blazor App on 5005.
-	Test the Log in, Log out feature in blazor app. You can use these two “awesome” accounts: alice/alice , bob/bob


# Questions & Answers

## 1. Which version of oidc-client-js is being used? Can I use a newer version?
- The current oidc-client version is **1.10.1**. I will consider updating this version in new library updates.
- If you think the oidc-client-js version is too old, you can completely replace the link in <script> tag with your own file. However, check it out carefully, because the new library may change API and make app broken.
**Note**: One trick to debug is: if you get an error related to the oidc-client library, replace the "oidc-client.min.js" src in my script tag with a link to oidc-client.js file on your project.
This way you can use an uncompressed file to aid in understanding the problem.
  
## 2. How many authentication flows are supported by the library?
- Because this library is a client-side authentication library, the number of supported flows will comply with the security rules of OAuth2 & OpenId Connect standards for browser applications. There are 2 supported flows:
+ Implicit flow
+ Authentication code.
You can download the source code for details on how to set up for specific cases, and for more details refer to the oidc-client-js wiki.

## 3. I saw in the wiki of oidc-client-js, there are a lot of configuration options when creating the UserManager object, how many of these are supported by HLSoft?
I always try to both simplify the use of the oidc-client and bring the most flexibility to user, so most of the possible configurations for the Oidc.UserManager constructor are provided by HLSoft, you can see these configs in the definition of 2 classes OpenIdConnectOptions & ClientOptions
If any configuration is missing and you find support for it necessary, please notify me.

## 4. Do I need to read oidc-client-js's documentation before using this library?
In normal use, you won't need to read it. However, if you need advanced configurations, or encounter errors that arise for unknown reasons, consult their documentation.

## 5. I saw that the way library parse claims is not suitable to my requirement. Is there any way for me to customize it?
Yes. Currently, HLSoft is using the DefaultClaimsParser class, which is an instance of IClaimsParser<TUser> interface, to parse claims. The current parsing algorithm is to use recursion to browse the structure of the User object that returned by the oidc-client.getUser() function, then flatten this structure into a list of claims.
However, if this method does not suit your application, you can customize it as follows:
### a. Extends DefaultClaimsParser class
When inheriting this class, you will have three methods that can be overridden:
```csharp
public virtual IIdentity CreateIdentity(object userClaims)
```
CreateIdentity is the method responsible for all claim conversion operations. userClaims is essentially a JsonElement object. You can override this function if your requirement involve the output object - IIdentity.
```csharp
protected virtual IList<Claim> ParseClaims(object userClaims)
```
This function converts the userClaims object into an array of Claims, which is also the place uses the recursive algorithm I describe above. If you have another algorithm, override this function!
```csharp
protected virtual void DetectClaimForIdentityName(IList<Claim> claims)
```
DetectClaimForIdentityName is the function responsible for finding a name claim to attach to User.Identity.Name. If the definition of this function is not exactly the way you want it to be, override it.
### b. Write a new ClaimsParser class
If you want to write new class, write a class that implements the IClaimsParser<TUser> interface
```csharp
public interface IClaimsParser<TUser> where TUser: class
{
    IIdentity CreateIdentity(TUser userClaims);
}
```
Here is a simple example
```csharp
public class User
{
  public string id_token { get; set; }
  public string session_state { get; set; }
  public string access_token { get; set; }
}
public class CustomClaimsParser : IClaimsParser<User>
{
  public IIdentity CreateIdentity(User user)
  {
    var claims = new List<Claim>();

    claims.Add(new Claim("access_token", user.access_token));
    claims.Add(new Claim("id_token", user.id_token));
    claims.Add(new Claim("session_state", user.session_state));

    return claims.Count == 0
      ? new ClaimsIdentity()
      : new ClaimsIdentity(claims, "Bearer");
  }
}
```
Pay attention to how I declare the User class. Here, instead of follow the way that DefaultClaimsParser class accepts TUser=object to handle raw data as JsonElement, I declare my own User class based on my knowledge on structure of the object returned by oidc-client.getUser(). And in the CustomClaimsParser class, I implement the CreateIdentity function in a simplest way to transfer values in the User object to a claims array.
You can see more detail in sample Client.IdentityServer.Code.Complex
**Note**: In both solutions, you must switch to the generic version of the AddBlazoredOpenIdConnect function in order to register the new IClaimsParser class:
```csharp
services.AddBlazoredOpenIdConnect<User, CustomClaimsParser>(options => {
  // .....
});
```
## 6. I think the way that library stores user claims data in sessionStorage that doesn't suit my needs. I want to replace it with localStorage, can I?
First of all, please remind that using localStorage/client cookie to store data such as access token is insecure (please read more on OAuth2 specialized websites)
If you still want to replace it, you can add the following code bellow script tags of HLSoft in index.html:
```javascript
<script>
	window.HLSoftBlazorWebAssemblyAuthenticationOpenIdConnect.configUserStore(window.localStorage);
</script>
```
Note that not only replace with localStorage, you can write a custom Storage and use it here if you want (perhaps you have a more secure storage than sessionStorage?).

## 7. How many login methods are supported?
There are 2 methods:
- Redirect to Identity Provider page
This method requires two methods of IAuthenticationService: SignInAsync and SignOutAsync
- Using popup.
Similarly, this method also uses two methods from IAuthenticationService: SignInPopupAsync, SignOutPopupAsync
You can implement 1 of 2 ways or both.

## 8. I want my application checks the login status right away and redirects user immediately if they are not already logged in, how do I do it?
Refer to the example project Client.IdentityServer.Implicit.RequiredLogin. In this example, I use the login mechanism you expect. The main method to invoke this login mechanism is IAuthenticationService.RequireAuthenticationAsync(). This method will check the user authentication status and automatically redirect the site if needed.
## 9. I see that library often log errors to console, is there any way to prevent it?
Yes, there is. You can set the configuration field WriteErrorToConsole to false. This configuration value is false by default, so the library will output errors to console.
## 10. I don't see any example project using Client Secret. Does the library not support ClientSecret?
For completeness, I do support ClientSecret, but I do not recommend that you provide ClientSecret in a web client application. If possible, avoid using this value. Implicit flow and Authentication flow both support logging in without Client Secret.

## 11. Hey, I got the error "no end session endpoint" when I logged out, how to fix it?
This error happens because the library cannot find the uri to do the logout function inside the identity provider's specification. To give you a better understanding, I will briefly explain how the logout function works.
The logout function includes two main jobs:
- Log out the user from the application (including clearing data storage, revoke token if needed, etc ....)
- Call an Api to log out the user from the Identity Provider. To do this, in the /.well-known/openid-configuration specification of Provider, there should be a field called end_session_endpoint.
This end_session_endpoint field contains the API url we need to call to sign out the Identity Provider
The problem we have here is that some identity providers do not have end_session_endpoint config field. This causes the oidc-client to fail and throw an exception like you saw.

I have had implementation to overcome this error. The first step you shoud do is, when you encounter this error, you need to set up a configuration item that serves as a replacement for end_session_endpoint as follows:
```csharp
options.EndSessionEndpoint = "/oauth0-logout";
```
Ok, this is step 1. You can stop here without error anymore. However, if you only do this step, you will not actually log out of the Identity Provider. That is, you will only log out of your application and ignore the call to the logout API on the Identity Provider
If you don't want to stop, ok, now you need to find out if your Identity Provider provides a logout API, and if so, what specification does the api offer?
If that API uses PUT or POST method, I think that's 90% you should give up because you will get a CORS error if you try to call it. Find out if your Identity Provider allows you to configure CORS for the logout link, if not, give up the remaining 10%.
Of course, there are still another solution for you to bypass CORS, such as using an intermediary server, but neither is easy or insecure.
In my example projects, there are some cases like this, and I stopped after setting the options.EndSessionEndpoint url.

There are still some cases where the Identity Provider allows it, such as the case of Auth0.com, the logout function is a GET method, and although there still has CORS limitation that doesn't allow me to use ajax, but because it is a GET method, I can use an intermediary iframe to run it. In this case I will declare another configuration, EndSessionEndpointProcess:
```csharp
options.EndSessionEndpointProcess = async provider =>
{
	var config = provider.GetService<ClientOptions>();
	var logoutUrl = $"{config.authority}/v2/logout";
	logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "client_id", config.client_id);
	logoutUrl = QueryHelpers.AddQueryString(logoutUrl, "returnTo", config.doNothingUri);
	var authenticationService = provider.GetService<IAuthenticationService>();
	await authenticationService.SilentOpenUrlInIframe(logoutUrl);
};
```
You can see how I avoid CORS using IAuthenticationService.SilentOpenUrlInIframe() method. This method will automatically create an iframe, load the url inside it, run and remove itself when done.

The guide above are the steps I have done to log out on Auth0.com. Because the Auth0.com Provider requires redirecting to another url after it's done its logout job, I've used a value called doNothingUri. This is a special url that does the same job as it suggests: "do nothing". I want to have a URL that does not affect the currently authentication process so I included the concept of doNothingUri here. If you encounter the same situation, use it!
Note that do not use AuthenticationStateProvider.GetAuthenticationStateAsync() inside the EndSessionEndpointProcess method because you will encounter an infinite loop if you call it. Moreover because of step 1 - logging out of the application - has finished before EndSessionEndpointProcess, GetAuthenticationStateAsync cannot return any useful data.

## 12. What do I need to configure on my Identity Provider so that my application can interact with it?
The answer depends on the support of each specific Identity Provider, and also the authentication method you want to use on the client side app.
- The first thing is to configure the Identity Provider to accept CORS. Because our app is an application that runs in browser, the first thing the browser complains about is CORS. You need to make sure you have configured the Provider to accept CORS from the client's domain.
- Next, you need to register the urls. Depending on the Identity Provider, these urls are classified into different lists. I will list these urls depending on the authentication method you use in the client app (remember to add domain and port to the url before register):
+ If you log in using redirect method, you will need 2 urls:
SignedOutRedirectUri (default: "/"): this is the link to redirect when logged out
SignedInCallbackUri (default: "/signin-callback-oidc"): link redirect back to the application when logging in
+ If using popup:
PopupSignOutRedirectUri: redirect link when logged out
PopupSignInRedirectUri: redirect link when logging in
+ If you use EndSessionEndpoint, remember to add this link
+ In a few cases, if you use DoNothingUri, remember to register it (default: "/oidc-nothing")
In addition, most providers will require you to register to add the domain of website.

## 13. I see you have lots of examples for Identity Server, does the library support Identity Server the most? How many other Identity Providers are supported?
Library has the best support for Identity Server. However, in theory, any Provider implements properly OpenId Connect will be supported. You can see I have many examples for other providers. However, the problem is that sometimes other providers do not provide end_session_endpoint and you need to code to solve it. You can see the solution for this problem above.
Another problem is that most providers only have good support for implicit flow, not authentication code flow, usually due to CORS errors


   [oidc-repo]: <https://github.com/IdentityModel/oidc-client-js>
   [oidc-doc]: <https://github.com/IdentityModel/oidc-client-js/wiki>
