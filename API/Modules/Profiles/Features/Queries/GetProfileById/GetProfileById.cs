using API.Common.CQRS;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Dtos;
using API.Modules.Profiles.Exceptions;
using API.Services.Interfaces;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Profiles.Features.Queries.GetProfileById;

public record GetProfileByIdQuery(Guid Id) : IQuery<GetProfileByIdResult>;

public record GetProfileByIdResult(ProfileDto Profile);

public record GetProfileByIdResponseDto(ProfileDto Profile);

public class GetProfileByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/profiles/{id:guid}", async (Guid id, ISender sender) =>
            {
                var query = new GetProfileByIdQuery(id);

                var result = await sender.Send(query);

                var response = result.Profile.Adapt<GetProfileByIdResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("GetProfileById")
            .Produces<GetProfileByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get a profile by id")
            .WithDescription("Get a profile by id");
    }
}

public class GetProfileByIdQueryValidator : AbstractValidator<GetProfileByIdQuery>
{
    public GetProfileByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal class GetProfileByIdQueryHandler(
    IProfilesRepository profilesRepository,
    IBlobStorageService blobStorageService)
    : IQueryHandler<GetProfileByIdQuery, GetProfileByIdResult>
{
    public async Task<GetProfileByIdResult> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var profile = await profilesRepository.GetProfileByIdWithSportsAsync(query.Id);
        if (profile == null)
            throw new ProfileNotFoundException(query.Id);

        var mainPhotoSasUrl = blobStorageService.GetBlobSasUrl(profile.MainPhotoUrl);
        profile.SetMainPhotoUrl(mainPhotoSasUrl);

        var profileDto = profile.Adapt<ProfileDto>();

        return new GetProfileByIdResult(profileDto);
    }
}