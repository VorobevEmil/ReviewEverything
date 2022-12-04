using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using ReviewEverything.Server.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ReviewEverything API", Version = "v1" });
});

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


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
