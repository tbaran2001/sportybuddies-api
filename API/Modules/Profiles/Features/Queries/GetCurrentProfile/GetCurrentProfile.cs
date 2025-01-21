using System.Security.Claims;
using API.Common.CQRS;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Dtos;
using API.Modules.Profiles.Exceptions;
using Ardalis.GuardClauses;
using Carter;
using Mapster;
using MediatR;

namespace API.Modules.Profiles.Features.Queries.GetCurrentProfile;

public record GetCurrentProfileQuery : IQuery<GetCurrentProfileResult>;

public record GetCurrentProfileResult(ProfileDto Profile);

public record GetCurrentProfileResponseDto(ProfileDto Profile);

public class GetCurrentProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/profiles/me", async (ISender sender) =>
            {
                var query = new GetCurrentProfileQuery();

                var result = await sender.Send(query);

                var response = result.Adapt<GetCurrentProfileResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("GetCurrentProfile")
            .Produces<GetCurrentProfileResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get current user profile")
            .WithDescription("Get the profile of the current user");
    }
}

internal class GetCurrentProfileQueryHandler(
    IProfilesRepository profilesRepository,
    ICurrentUserProvider currentUserProvider) : IQueryHandler<GetCurrentProfileQuery, GetCurrentProfileResult>
{
    public async Task<GetCurrentProfileResult> Handle(GetCurrentProfileQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var currentUserId = currentUserProvider.GetCurrentUserId();

        var profile = await profilesRepository.GetProfileByIdWithSportsAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        var profileDto = profile.Adapt<ProfileDto>();

        return new GetCurrentProfileResult(profileDto);
    }
}