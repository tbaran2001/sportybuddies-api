namespace API.Hubs;

public record HubMessage(
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTime CreatedAt,
    List<Guid> Participants);