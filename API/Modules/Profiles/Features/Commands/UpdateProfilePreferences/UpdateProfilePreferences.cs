using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Enums;
using API.Modules.Profiles.Exceptions;
using API.Modules.Profiles.ValueObjects;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Profiles.Features.Commands.UpdateProfilePreferences;

public record UpdateProfilePreferencesCommand(int MinAge, int MaxAge, int MaxDistance, Gender PreferredGender)
    : ICommand;

public record UpdateProfilePreferencesRequestDto(int MinAge, int MaxAge, int MaxDistance, Gender PreferredGender);

public class UpdateProfilePreferencesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/profiles/preferences", async (UpdateProfilePreferencesRequestDto request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProfilePreferencesCommand>();

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("UpdateProfilePreferences")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update profile preferences")
            .WithDescription("Update profile preferences of the current user");
    }
}

public class UpdateProfilePreferencesCommandValidator : AbstractValidator<UpdateProfilePreferencesCommand>
{
    public UpdateProfilePreferencesCommandValidator()
    {
        RuleFor(x => x.MinAge)
            .InclusiveBetween(18, 100);
        RuleFor(x => x.MaxAge)
            .InclusiveBetween(18, 100);
        RuleFor(x => x.MaxDistance)
            .InclusiveBetween(1, 100);
        RuleFor(x => x.PreferredGender)
            .IsInEnum();
    }
}

internal class UpdateProfilePreferencesCommandHandler(
    IProfilesRepository profilesRepository,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProfilePreferencesCommand>
{
    public async Task<Unit> Handle(UpdateProfilePreferencesCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var profile = await profilesRepository.GetProfileByIdAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        profile.UpdatePreferences(Preferences.Create(command.MinAge, command.MaxAge, command.MaxDistance,
            command.PreferredGender));
        await unitOfWork.CommitChangesAsync();

        return Unit.Value;
    }
}