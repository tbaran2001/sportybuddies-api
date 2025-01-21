using API.Modules.Conversations.Models;

namespace API.Data.Repositories.Interfaces;

public interface IConversationsRepository
{
    Task AddConversationAsync(Conversation conversation);
    Task<Conversation> GetConversationByIdAsync(Guid id);
    Task AddMessageAsync(Message message);
    Task<Conversation> GetConversationByIdWithMessagesAsync(Guid conversationId);
    Task<IEnumerable<Message>> GetLastMessageFromEachProfileConversationAsync(Guid profileId);
    Task<bool> ProfilesHaveConversationAsync(Guid firstProfileId, Guid secondProfileId);
}