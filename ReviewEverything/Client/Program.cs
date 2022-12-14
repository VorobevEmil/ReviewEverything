using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ReviewEverything.Client;
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

builder.Services.AddAuthorizationCore(configure =>
{
    configure.AddPolicy("Admin", pb =>
    {
        pb.RequireAuthenticatedUser()
            .RequireRole(new[] { "Admin" });
    });
});

await builder.Build().RunAsync();
