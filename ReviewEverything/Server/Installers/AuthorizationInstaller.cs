using ReviewEverything.Server.Common.Policies;
using ReviewEverything.Server.Services.ReviewService;

namespace ReviewEverything.Server.Installers
{
    public class AuthorizationInstaller : IInstaller
    {
        public void InstallerServices(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(configure =>
            {
                configure.AddPolicy("Admin", pb =>
                {
                    pb.RequireAuthenticatedUser()
                        .RequireRole(new[] { "Admin" });
                });

                configure.AddPolicy("ChangingArticle", pb =>
                {
                    var serviceProvider = builder.Services!.BuildServiceProvider()!;
                    pb.RequireAuthenticatedUser()
                        .AddRequirements(new PossibilityChangingArticleHandler(serviceProvider.GetService<IReviewService>()!, serviceProvider.GetService<IHttpContextAccessor>()!));
                });
            });
        }
    }
}