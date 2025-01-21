using API.Common.Exceptions;

namespace API.Modules.Matches.Exceptions;

public class RandomMatchNotFoundException() : NotFoundException("Random match not found");