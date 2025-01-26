using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Exceptions;
using API.Modules.Profiles.ValueObjects;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Profiles.Features.Commands.UpdateProfileLocation;

public record UpdateProfileLocationCommand(double Latitude, double Longitude, string Address) : ICommand;

public record UpdateProfileLocationRequestDto(double Latitude, double Longitude, string Address);

public class UpdateProfileLocationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/profiles/location", async (UpdateProfileLocationRequestDto request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProfileLocationCommand>();

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("UpdateProfileLocation")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update profile location")
            .WithDescription("Update profile location of the current user");
    }
}

public class UpdateProfileLocationCommandValidator : AbstractValidator<UpdateProfileLocationCommand>
{
    public UpdateProfileLocationCommandValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}

internal class UpdateProfileLocationCommandHandler(
    IProfilesRepository profilesRepository,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProfileLocationCommand>
{
    public async Task<Unit> Handle(UpdateProfileLocationCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var profile = await profilesRepository.GetProfileByIdWithSportsAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        var location = Location.Create(command.Latitude, command.Longitude, command.Address);

        profile.UpdateLocation(location);
        await unitOfWork.CommitChangesAsync();

        return Unit.Value;
    }
}