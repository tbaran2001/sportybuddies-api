namespace API.Modules.Conversations.Dtos;

public record ConversationDto(Guid Id, Guid CreatorId, List<ParticipantDto> Participants);