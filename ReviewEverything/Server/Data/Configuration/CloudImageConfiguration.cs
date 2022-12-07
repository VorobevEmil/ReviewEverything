using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data.Configuration
{
    public class CloudImageConfiguration : IEntityTypeConfiguration<CloudImage>
    {
        public void Configure(EntityTypeBuilder<CloudImage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);

            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Url).IsRequired();

            builder.HasOne(left => left.Review)
                .WithMany(right => right.CloudImages)
                .HasForeignKey(left => left.ReviewId);
        }
    }
}
