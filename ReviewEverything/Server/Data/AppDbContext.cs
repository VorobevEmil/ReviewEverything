using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data.Configuration;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<CloudImage> CloudImages { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Composition> Compositions { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<UserScore> UserScores { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CompositionConfiguration());
            builder.ApplyConfiguration(new UserScoreConfiguration());
            builder.ApplyConfiguration(new ReviewConfiguration());
            builder.ApplyConfiguration(new CloudImageConfiguration());
            builder.ApplyConfiguration(new TagConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());
            builder.ApplyConfiguration(new ApplicationUserConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
