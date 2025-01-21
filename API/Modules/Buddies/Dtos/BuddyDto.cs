namespace API.Modules.Buddies.Dtos;

public record BuddyDto(Guid Id, Guid OppositeBuddyId, Guid ProfileId, Guid MatchedProfileId, DateTime CreatedOn);