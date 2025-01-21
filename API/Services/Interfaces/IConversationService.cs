using API.Modules.Conversations.Models;

namespace API.Services.Interfaces;

public interface IConversationService
{
    Task<Conversation> CreateConversationAsync(Guid creatorId, Guid participantId);
}