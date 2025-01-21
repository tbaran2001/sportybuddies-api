using API.Common.CQRS;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Buddies.Dtos;
using Ardalis.GuardClauses;
using Carter;
using Mapster;
using MediatR;

namespace API.Modules.Buddies.Features.GetProfileBuddies;

public record GetProfileBuddiesQuery : IQuery<GetProfileBuddiesResult>;

public record GetProfileBuddiesResult(IEnumerable<BuddyDto> Buddies);

public record GetProfileBuddiesResponseDto(IEnumerable<BuddyDto> Buddies);

public class GetProfileBuddiesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/buddies", async (ISender sender) =>
            {
                var query = new GetProfileBuddiesQuery();

                var result = await sender.Send(query);

                var response = result.Adapt<GetProfileBuddiesResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Buddies")
            .WithName("GetProfileBuddies")
            .Produces<GetProfileBuddiesResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get profile buddies")
            .WithDescription("Get profile buddies of the current user");
    }
}

internal class GetProfileBuddiesQueryHandler(
    IBuddiesRepository buddiesRepository,
    ICurrentUserProvider currentUserProvider)
    : IQueryHandler<GetProfileBuddiesQuery, GetProfileBuddiesResult>
{
    public async Task<GetProfileBuddiesResult> Handle(GetProfileBuddiesQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var buddies = await buddiesRepository.GetProfileBuddiesAsync(currentUserId);

        var buddyDtos = buddies.Adapt<IEnumerable<BuddyDto>>();

        return new GetProfileBuddiesResult(buddyDtos);
    }
}