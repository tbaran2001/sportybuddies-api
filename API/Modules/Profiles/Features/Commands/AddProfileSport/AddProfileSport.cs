using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Exceptions;
using API.Modules.Sports.Exceptions;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using MediatR;

namespace API.Modules.Profiles.Features.Commands.AddProfileSport;

public record AddProfileSportCommand(Guid SportId) : ICommand;

public record ProfileSportAddedDomainEvent(Guid ProfileId, IEnumerable<Guid> SportIds) : IDomainEvent;

public class AddProfileSportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profiles/sports/{sportId:guid}", async (Guid sportId, ISender sender) =>
            {
                var command = new AddProfileSportCommand(sportId);

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("AddProfileSport")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add a sport to a profile")
            .WithDescription("Add a sport to a profile");
    }
}

public class AddProfileSportCommandValidator : AbstractValidator<AddProfileSportCommand>
{
    public AddProfileSportCommandValidator()
    {
        RuleFor(x => x.SportId).NotEmpty();
    }
}

internal class AddProfileSportCommandHandler(
    IProfilesRepository profilesRepository,
    ISportsRepository sportsRepository,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<AddProfileSportCommand>
{
    public async Task<Unit> Handle(AddProfileSportCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var profile = await profilesRepository.GetProfileByIdWithSportsAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        var sport = await sportsRepository.GetSportByIdAsync(command.SportId);
        if (sport == null)
            throw new SportNotFoundException(command.SportId);

        profile.AddSport(sport);
        await unitOfWork.CommitChangesAsync();

        return Unit.Value;
    }
}