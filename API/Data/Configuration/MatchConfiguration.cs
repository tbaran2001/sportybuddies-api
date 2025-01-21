using API.Modules.Matches.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuration;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder
            .HasOne(m => m.Profile)
            .WithMany()
            .HasForeignKey(m => m.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(m => m.MatchedProfile)
            .WithMany()
            .HasForeignKey(m => m.MatchedProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}