using API.Common.Exceptions;

namespace API.Modules.Matches.Exceptions;

public class MatchesNotFoundException(Guid profileId)
    : NotFoundException($"Matches not found for profile with ID: {profileId}");