using API.Modules.Profiles.Enums;

namespace API.Modules.Identity;

public sealed class CustomRegisterRequest
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }

    public required string Name { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required Gender Gender { get; init; }
}