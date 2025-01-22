using API.Common.CQRS;
using API.Common.Web;
using API.Modules.Matches.Dtos;
using API.Modules.Matches.Exceptions;
using API.Services.Interfaces;
using Ardalis.GuardClauses;
using Carter;
using Mapster;
using MediatR;

namespace API.Modules.Matches.Features.Queries.GetRandomMatch;

public record GetRandomMatchQuery : IQuery<GetRandomMatchResult>;

public record GetRandomMatchResult(MatchDto Match);

public record GetRandomMatchResponseDto(MatchDto Match);

public class GetRandomMatchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/matches/get-random-match", async (ISender sender) =>
            {
                var query = new GetRandomMatchQuery();

                var result = await sender.Send(query);

                var response = result.Adapt<GetRandomMatchResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Matches")
            .WithName("GetRandomMatch")
            .Produces<GetRandomMatchResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get random match for a current profile")
            .WithDescription("Get a random match for a profile by current profile id");
    }
}

internal class GetRandomMatchQueryHandler(
    IMatchService matchService,
    ICurrentUserProvider currentUserProvider)
    : IQueryHandler<GetRandomMatchQuery, GetRandomMatchResult>
{
    public async Task<GetRandomMatchResult> Handle(GetRandomMatchQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var matchDto = await matchService.GetRandomMatchAsync(currentUserId);
        if (matchDto is null)
            throw new RandomMatchNotFoundException();

        return new GetRandomMatchResult(matchDto);
    }
}