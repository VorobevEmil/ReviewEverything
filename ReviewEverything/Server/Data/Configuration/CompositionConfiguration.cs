using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Data.Configuration
{
    public class CompositionConfiguration : IEntityTypeConfiguration<Composition>
    {
        public void Configure(EntityTypeBuilder<Composition> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);

            builder.Property(x => x.Title).IsRequired();

            builder.HasOne(left => left.Category)
                .WithMany(right => right.Compositions)
                .HasForeignKey(left => left.CategoryId);

            builder.HasMany(left => left.UserScores)
                .WithOne(right => right.Composition)
                .HasForeignKey(right => right.CompositionId);

            builder.HasMany(left => left.Reviews)
                .WithOne(right => right.Composition)
                .HasForeignKey(right => right.CompositionId);
        }
    }
}
