using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);

            builder.HasAlternateKey(x => new { x.ReviewId, x.UserId });

            builder.Property(x => x.Body).IsRequired();

            builder.HasOne(left => left.Review)
                .WithMany(right => right.Comments)
                .HasForeignKey(left => left.ReviewId);

            builder.HasOne(left => left.User)
                .WithMany(right => right.Comments)
                .HasForeignKey(left => left.UserId);

            builder.HasMany(left => left.LikeUsers)
                .WithMany(right => right.LikeComments)
                .UsingEntity(entity => entity.ToTable("Likes"));
        }
    }
}
