using API.Modules.Matches.Enums;
using API.Modules.Profiles.Dtos;

namespace API.Modules.Matches.Dtos;

public record MatchDto(
    Guid Id,
    Guid OppositeMatchId,
    Guid ProfileId,
    ProfileDto MatchedProfile,
    DateTime MatchDateTime,
    Swipe? Swipe,
    DateTime? SwipeDateTime,
    double Distance);