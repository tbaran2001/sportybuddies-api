using API.Common.CQRS;
using API.Common.Models;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Dtos;
using API.Modules.Profiles.Enums;
using API.Modules.Profiles.Models;
using Ardalis.GuardClauses;
using FluentValidation;
using Mapster;

namespace API.Modules.Profiles.Features.Commands.CreateProfile;

public record CreateProfileCommand(Guid ProfileId, string Name, DateOnly DateOfBirth, Gender Gender)
    : ICommand<CreateProfileResult>;

public record CreateProfileResult(ProfileDto Profile);

public record ProfileCreatedDomainEvent(Guid ProfileId) : IDomainEvent;

public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.DateOfBirth)
            .Must(dateOfBirth => DateTime.Now.Year - dateOfBirth.Year >= 18);
        RuleFor(x => x.Gender)
            .IsInEnum();
    }
}

internal class CreateProfileCommandHandler(IProfilesRepository profilesRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProfileCommand, CreateProfileResult>
{
    public async Task<CreateProfileResult> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var profile = Profile.Create(
            command.ProfileId,
            command.Name,
            command.DateOfBirth,
            command.Gender
        );

        await profilesRepository.AddProfileAsync(profile);
        await unitOfWork.CommitChangesAsync();

        var profileDto = profile.Adapt<ProfileDto>();

        return new CreateProfileResult(profileDto);
    }
}