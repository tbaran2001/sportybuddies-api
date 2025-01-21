using API.Modules.Matches.Enums;

namespace API.Modules.Matches.Dtos;

public record MatchDto(
    Guid Id,
    Guid OppositeMatchId,
    Guid ProfileId,
    Guid MatchedProfileId,
    DateTime MatchDateTime,
    Swipe Swipe,
    DateTime SwipeDateTime);