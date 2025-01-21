using API.Common.CQRS;
using API.Data.Repositories.Interfaces;
using API.Modules.Matches.Dtos;
using API.Modules.Matches.Exceptions;
using Carter;
using Mapster;
using MediatR;

namespace API.Modules.Matches.Features.Queries.GetProfileMatches;

public record GetProfileMatchesQuery(Guid ProfileId) : IQuery<GetProfileMatchesResult>;

public record GetProfileMatchesResult(IEnumerable<MatchDto> Matches);

public record GetProfileMatchesResponseDto(IEnumerable<MatchDto> Matches);

public class GetProfileMatchesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/matches/{profileId}", async (Guid profileId, ISender sender) =>
            {
                var query = new GetProfileMatchesQuery(profileId);

                var result = await sender.Send(query);

                var response = result.Adapt<GetProfileMatchesResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Matches")
            .WithName("GetProfileMatches")
            .Produces<GetProfileMatchesResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get matches for a profile")
            .WithDescription("Get all matches for a profile by profile id");
    }
}

internal class GetProfileMatchesQueryHandler(
    IMatchesRepository matchesRepository)
    : IQueryHandler<GetProfileMatchesQuery, GetProfileMatchesResult>
{
    public async Task<GetProfileMatchesResult> Handle(GetProfileMatchesQuery query, CancellationToken cancellationToken)
    {
        var matches = await matchesRepository.GetProfileMatchesAsync(query.ProfileId);
        if (matches is null)
            throw new MatchesNotFoundException(query.ProfileId);

        var matchDtos = matches.Adapt<IEnumerable<MatchDto>>();

        return new GetProfileMatchesResult(matchDtos);
    }
}