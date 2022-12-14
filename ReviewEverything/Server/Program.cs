using Microsoft.AspNetCore.HttpOverrides;
using ReviewEverything.Server.Options;
using ReviewEverything.Server.Installers;

var builder = WebApplication.CreateBuilder(args);

builder.InstallServicesInAssembly();

builder.Services.AddAuthentication()
    .AddGoogle(opt =>
    {
        opt.ClientId = "751529687127-bba95tajqo5qm9qjf37mqmm1thttu5pj.apps.googleusercontent.com";
        opt.ClientSecret = "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";
    });
builder.Services.AddAuthorization(configure =>
{
    configure.AddPolicy("Admin", pb =>
    {
        pb.RequireAuthenticatedUser()
            .RequireRole(new[] { "Admin" });
    });
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