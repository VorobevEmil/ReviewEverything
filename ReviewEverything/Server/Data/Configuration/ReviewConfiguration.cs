using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data.Configuration
{
    public class ReviewConfiguration: IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);

            builder.HasAlternateKey(x => new { x.CompositionId, x.AuthorId });

            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Body).IsRequired();
            builder.Property(x => x.AuthorScore).IsRequired();
            builder.Property(x => x.CreationDate).IsRequired();

            builder.HasMany(left => left.CloudImages)
                .WithOne(right => right.Review)
                .HasForeignKey(right => right.ReviewId);

            builder.HasOne(left => left.Author)
                .WithMany(right => right.AuthorReviews)
                .HasForeignKey(right => right.AuthorId);

            builder.HasOne(left => left.Composition)
                .WithMany(right => right.Reviews)
                .HasForeignKey(right => right.CompositionId);

            builder.HasMany(left => left.Tags)
                .WithMany(right => right.Reviews)
                .UsingEntity(entity => entity.ToTable("ReviewTags"));


            builder.HasMany(left => left.LikeUsers)
                .WithMany(right => right.LikeReviews)
                .UsingEntity(entity => entity.ToTable("Likes"));
        }
    }
}
