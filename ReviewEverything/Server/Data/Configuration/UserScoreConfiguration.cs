using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReviewEverything.Server.Data.Configuration
{
    internal class UserScoreConfiguration : IEntityTypeConfiguration<UserScore>
    {
        public void Configure(EntityTypeBuilder<UserScore> builder)
        {
            builder.HasKey(x => new { x.CompositionId, x.UserId });

            builder.Property(x => x.Score).IsRequired();

            builder.HasOne(left => left.Composition)
                .WithMany(right => right.UserScores)
                .HasForeignKey(left => left.CompositionId);

            builder.HasOne(left => left.User)
                .WithMany(right => right.UserScores)
                .HasForeignKey(left => left.UserId);
        }
    }
}
