using Microsoft.OpenApi.Models;

namespace ReviewEverything.Server.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallerServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ReviewEverything API", Version = "v1" });
            });
        }
    }
}