using Microsoft.AspNetCore.HttpOverrides;
using ReviewEverything.Server.Options;
using ReviewEverything.Server.Installers;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.InstallServicesInAssembly();

builder.Services.AddAuthentication()
    .AddGoogle(opt =>
    {
        opt.ClientId = configuration["Authentication:Google:ClientId"]!;
        opt.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddVkontakte(opt =>
    {
        opt.ClientId = configuration["Authentication:Vkontakte:ClientId"]!;
        opt.ClientSecret = configuration["Authentication:Vkontakte:ClientSecret"]!;
    });

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    var swaggerOptions = new SwaggerOptions();
    app.Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
    app.UseSwagger(options =>
    {
        options.RouteTemplate = swaggerOptions.JsonRoute;
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(swaggerOptions.UiEndPoint, swaggerOptions.Description);
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();