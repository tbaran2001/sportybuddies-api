using API.Common.CQRS;
using API.Common.Models;
using API.Common.Web;
using API.Data.Repositories.Interfaces;
using API.Modules.Profiles.Exceptions;
using API.Services.Interfaces;
using Ardalis.GuardClauses;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;

namespace API.Modules.Profiles.Features.Commands.UploadProfilePhoto;

public record UploadProfilePhotoCommand(Stream File, string FileName) : ICommand<UploadProfilePhotoResult>;

public record UploadProfilePhotoResult(string PhotoUrl);

public record UploadProfilePhotoRequestDto(Stream File, string FileName);

public record UploadProfilePhotoResponseDto(string PhotoUrl);

public class UploadProfilePhotoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profiles/me/photo", async (UploadProfilePhotoRequestDto request, ISender sender) =>
            {
                var command = request.Adapt<UploadProfilePhotoCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<UploadProfilePhotoResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Profiles")
            .WithName("UploadProfilePhoto")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload a profile photo")
            .WithDescription("Upload a photo for the profile of the current user");
    }
}

public class UploadProfilePhotoCommandValidator : AbstractValidator<UploadProfilePhotoCommand>
{
    public UploadProfilePhotoCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull();
        RuleFor(x => x.FileName)
            .NotEmpty();
    }
}

internal class UploadProfilePhotoCommandHandler(
    IProfilesRepository profilesRepository,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork,
    IProfilePhotoService profilePhotoService) : ICommandHandler<UploadProfilePhotoCommand, UploadProfilePhotoResult>
{
    public async Task<UploadProfilePhotoResult> Handle(UploadProfilePhotoCommand command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var currentUserId = currentUserProvider.GetCurrentUserId();
        var profile = await profilesRepository.GetProfileByIdAsync(currentUserId);
        if (profile == null)
            throw new ProfileNotFoundException(currentUserId);

        var photoUrl =
            await profilePhotoService.UploadAndAssignPhotoAsync(profile, command.File, command.FileName, true);
        await unitOfWork.CommitChangesAsync();

        return new UploadProfilePhotoResult(photoUrl);
    }
}