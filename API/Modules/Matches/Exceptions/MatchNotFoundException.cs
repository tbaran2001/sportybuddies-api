using API.Common.Exceptions;

namespace API.Modules.Matches.Exceptions;

public class MatchNotFoundException(Guid matchId) : NotFoundException("Match", matchId);