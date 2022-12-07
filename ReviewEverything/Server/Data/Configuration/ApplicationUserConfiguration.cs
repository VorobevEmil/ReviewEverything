using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data.Configuration
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(left => left.AuthorReviews)
                .WithOne(right => right.Author)
                .HasForeignKey(right => right.AuthorId);

            builder.HasMany(left => left.Comments)
                .WithOne(right => right.User)
                .HasForeignKey(right => right.UserId);

            builder.HasMany(left => left.LikeComments)
                .WithMany(right => right.LikeUsers)
                .UsingEntity(entity => entity.ToTable("Likes"));

            builder.HasMany(left => left.UserScores)
                .WithOne(right => right.User)
                .HasForeignKey(right => right.UserId);
        }
    }
}
