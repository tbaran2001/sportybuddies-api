using API.Modules.Profiles.Enums;
using API.Modules.Profiles.ValueObjects;
using API.Modules.Sports.Dtos;

namespace API.Modules.Profiles.Dtos;

public record ProfileDto(
    Guid Id,
    string Name,
    string Description,
    string MainPhotoUrl,
    DateTime CreatedOnUtc,
    Gender Gender,
    DateOnly DateOfBirth,
    Preferences Preferences,
    Location Location,
    List<SportDto> Sports);