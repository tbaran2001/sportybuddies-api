using API.Common.Models;
using API.Modules.Profiles.Models;

namespace API.Modules.Conversations.Models;

public class Participant : Entity
{
    public Guid ConversationId { get; private set; }
    public Guid ProfileId { get; private set; }
    public DateTime CreatedOn { get; private set; }

    public Conversation Conversation { get; private set; }
    public Profile Profile { get; private set; }

    public static Participant Create(Guid conversationId, Guid profileId)
    {
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            ProfileId = profileId,
            CreatedOn = DateTime.UtcNow
        };

        return participant;
    }
}