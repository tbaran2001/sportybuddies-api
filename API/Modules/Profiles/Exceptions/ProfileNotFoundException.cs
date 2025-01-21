
using API.Common.Exceptions;

namespace API.Modules.Profiles.Exceptions;

public class ProfileNotFoundException(Guid id) : NotFoundException("Profile", id);