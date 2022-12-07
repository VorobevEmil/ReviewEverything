using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data.Configuration
{
    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);

            builder.Property(x => x.Title).IsRequired();

            builder.HasMany(left => left.Reviews)
                .WithMany(right => right.Tags)
                .UsingEntity(entity => entity.ToTable("ReviewTags"));
        }
    }
}
