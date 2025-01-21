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

namespace API.Modules.Profiles.Features.Commands.RemoveProfileSport;

public record RemoveProfileSportCommand(Guid SportId) : ICommand;

public record ProfileSportRemovedDomainEvent(Guid ProfileId, IEnumerable<Guid> SportIds) : IDomainEvent;

public class RemoveProfileSportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("profiles/sports/{sportId:guid}", async (Guid sportId, ISender sender) =>
            {
                var command = new RemoveProfileSportCommand(sportId);

                await sender.Send(command);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithName("RemoveProfileSport")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Remove a sport from a profile")
            .WithDescription("Remove a sport from a profile");
    }
}

public class RemoveProfileSportCommandValidator : AbstractValidator<RemoveProfileSportCommand>
{
    public RemoveProfileSportCommandValidator()
    {
        RuleFor(x => x.SportId).NotEmpty();
    }
}

internal class RemoveProfileSportCommandHandler(
    IProfilesRepository profilesRepository,
    ISportsRepository sportsRepository,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveProfileSportCommand>
{
    public async Task<Unit> Handle(RemoveProfileSportCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var profile = await profilesRepository.GetProfileByIdWithSportsAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        var sport = await sportsRepository.GetSportByIdAsync(command.SportId);
        if (sport == null)
            throw new SportNotFoundException(command.SportId);

        profile.RemoveSport(sport);
        await unitOfWork.CommitChangesAsync();

        return Unit.Value;
    }
}