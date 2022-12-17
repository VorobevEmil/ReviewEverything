namespace ReviewEverything.Server.Installers
{
    public class AuthenticationInstaller : IInstaller
    {
        public void InstallerServices(WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

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
        }
    }
}
