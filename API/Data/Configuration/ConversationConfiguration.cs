using API.Modules.Conversations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuration;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);

        builder
            .HasMany(c => c.Participants)
            .WithOne(p => p.Conversation)
            .HasForeignKey(p => p.ConversationId);
    }
}