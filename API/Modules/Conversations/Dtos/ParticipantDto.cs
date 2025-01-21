using API.Modules.Profiles.Dtos;

namespace API.Modules.Conversations.Dtos;

public record ParticipantDto(Guid Id, Guid ConversationId, ProfileDto Profile, DateTime CreatedOn);