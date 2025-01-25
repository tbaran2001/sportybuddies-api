namespace API.Hubs;

public record HubMessage(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTime CreatedOn,
    List<Guid> Participants);