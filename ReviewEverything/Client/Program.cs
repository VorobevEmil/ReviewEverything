using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ReviewEverything.Client;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Client.Services;
using ReviewEverything.Client.Services.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<HostAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<HostAuthenticationStateProvider>());
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<DisplayHelper>();
builder.Services.AddScoped<BrowserService>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddAuthorizationCore(configure =>
{
    configure.AddPolicy("Admin", pb =>
    {
        pb.RequireAuthenticatedUser()
            .RequireRole("Admin");
    });
});


builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
var host = builder.Build();
await host.SetDefaultCulture();

await host.RunAsync();
