using API.Modules.Conversations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuration;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder
            .HasOne(p => p.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Profile)
            .WithMany()
            .HasForeignKey(p => p.ProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}