using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.CategoryService;
using ReviewEverything.Server.Services.CloudImageService;
using ReviewEverything.Server.Services.CommentService;
using ReviewEverything.Server.Services.CompositionService;
using ReviewEverything.Server.Services.ReviewService;
using ReviewEverything.Server.Services.TagService;
using ReviewEverything.Server.Services.UserLikeService;
using ReviewEverything.Server.Services.UserManagementService;
using ReviewEverything.Server.Services.UserScoreService;
using ReviewEverything.Server.Services.UserService;

namespace ReviewEverything.Server.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallerServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(builder.Configuration["ConnectionStrings:PostgreSQL"]));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICompositionService, CompositionService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IUserScoreService, UserScoreService>();
            builder.Services.AddScoped<IUserLikeService, UserLikeService>();
            builder.Services.AddScoped<IUserManagementService, UserManagementService>();
            builder.Services.AddScoped<ICloudImageService, CloudImageService>();
        }
    }
}