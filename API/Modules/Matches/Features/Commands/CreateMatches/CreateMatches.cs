using API.Common.CQRS;
using API.Common.Models;
using API.Data.Repositories.Interfaces;
using API.Modules.Matches.Models;
using Ardalis.GuardClauses;
using MediatR;

namespace API.Modules.Matches.Features.Commands.CreateMatches;

public record CreateMatchesCommand(Guid ProfileId, Guid MatchedProfileId) : ICommand;

public class CreateMatchesCommandHandler(IMatchesRepository matchesRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateMatchesCommand>
{
    public async Task<Unit> Handle(CreateMatchesCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        if (await matchesRepository.CheckIfMatchExistsAsync(command.ProfileId, command.MatchedProfileId))
            return Unit.Value;

        var (match1, match2) = Match.CreatePair(command.ProfileId, command.MatchedProfileId, DateTime.Now);

        var matches = new List<Match> { match1, match2 };

        await matchesRepository.AddMatchesAsync(matches);
        await unitOfWork.CommitChangesAsync();

        return Unit.Value;
    }
}