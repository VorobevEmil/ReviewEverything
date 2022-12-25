using Microsoft.AspNetCore.HttpOverrides;
using ReviewEverything.Server.Hubs;
using ReviewEverything.Server.Options;
using ReviewEverything.Server.Installers;

var builder = WebApplication.CreateBuilder(args);

builder.InstallServicesInAssembly();

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
app.MapHub<CommentHub>("/commentHub");
app.MapHub<UserManagerHub>("/userManagerHub");

app.MapFallbackToFile("index.html");

app.Run();