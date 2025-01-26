using API.Modules.Profiles.Dtos;

namespace API.Modules.Buddies.Dtos;

public record BuddyDto(
    Guid Id,
    Guid OppositeBuddyId,
    Guid ProfileId,
    ProfileDto MatchedProfile,
    DateTime CreatedOn,
    Guid? ConversationId);