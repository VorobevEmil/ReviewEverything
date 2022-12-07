using Microsoft.AspNetCore.HttpOverrides;
using ReviewEverything.Server.Options;
using ReviewEverything.Server.Installers;

var builder = WebApplication.CreateBuilder(args);

builder.InstallServicesInAssembly();

builder.Services.AddAuthentication()
    .AddGoogle(opt =>
    {
        opt.ClientId = "916919818119-ndfp1794mkuq4gghh9r1js73ntebfdmi.apps.googleusercontent.com";
        opt.ClientSecret = "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";
    });

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(Program));

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

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();