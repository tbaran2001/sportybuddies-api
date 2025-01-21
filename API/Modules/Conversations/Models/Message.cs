using API.Common.Models;
using API.Modules.Profiles.Models;

namespace API.Modules.Conversations.Models;

public class Message : Entity
{
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public Conversation Conversation { get; private set; }
    public Profile Sender { get; private set; }

    public static Message Create(Guid conversationId, Guid senderId, string content)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            CreatedOnUtc = DateTime.UtcNow
        };

        return message;
    }
}